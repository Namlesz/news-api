using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewsApp.api.Context;
using NewsApp.api.Models;
using NewsApp.api.Services;
using NewsApp.api.Settings;

namespace NewsApp.api.Controllers;

[ApiController]
[Route("[action]")]
public class AuthenticateController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthenticateController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    
    /// <summary>
    /// Get access token
    /// </summary>
    /// <response code="200">Return user and access token.</response>
    /// <response code="401">Email not confirmed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Ops! Uncaught error.</response>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user is null)
            return NotFound();

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized("Email not confirmed");

        if (!await _userManager.CheckPasswordAsync(user, login.Password))
            return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GetToken(authClaims);
        return Ok(new
        {
            userId = user.Id,
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            editorialOfficeId = user.EditorialOfficeId
        });
    }

    /// <summary>
    /// Create editor for redaction
    /// </summary>
    /// <response code="200">User created.</response>
    /// <response code="404">Not found owner/ Owner office is missing.</response>
    /// <response code="409">User already exists.</response>
    /// <response code="500">Ops! Can't create user.</response>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> RegisterEditor([FromQuery] string ownerId, [FromBody] NewUser data)
    {
        if (await _userManager.FindByEmailAsync(data.Email) != null)
            return Conflict("User already exists!");

        string editorialOfficeId;
        try
        {
            var owner = await _userManager.FindByIdAsync(ownerId);
            if (owner is null)
                return NotFound("Owner not found");
            if (owner.EditorialOfficeId is null)
                return NotFound("Owner has no editorial office");
            
            editorialOfficeId = owner.EditorialOfficeId;
        }
        catch
        {
            return Problem("Invalid owner id");
        }

        var user = await CreateUser(data, editorialOfficeId);
        if (user is null)
            return Problem("User creation failed! Please check user details and try again.");

        if (await _roleManager.RoleExistsAsync(UserRoles.Editor))
        { 
            await _userManager.AddToRoleAsync(user, UserRoles.Editor);
        }
        
        if (!await SendConfirmationEmail(user))
            return Problem("Confirmation email failed to send.");

        return Ok("User created successfully!");
    }

    /// <summary>
    /// Create new account
    /// </summary>
    /// <response code="200">User created.</response>
    /// <response code="409">User already exists.</response>
    /// <response code="500">Ops! Can't create user.</response>
    [HttpPost]
    public async Task<IActionResult> RegisterAdmin([FromBody] NewUser data)
    {
        if (await _userManager.FindByEmailAsync(data.Email) != null)
            return Conflict("User already exists!");
        
        var user = await CreateUser(data);
        if (user is null)
            return Problem("User creation failed! Please check user details and try again.");

        foreach (var role in Enum.GetNames(typeof(Roles)))
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        if (!await SendConfirmationEmail(user))
            return Problem("Confirmation email failed to send.");

        return Ok("User created successfully! Pleas confirm your email.");
    }

    /// <summary>
    /// Change password for account
    /// </summary>
    /// <response code="200">Password changed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Ops! Can't change password.</response>
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordChange data)
    {
        var user = await _userManager.FindByEmailAsync(data.Email);
        if (user == null)
            return NotFound("User not found");

        var result = await _userManager.ResetPasswordAsync(user, data.Token, data.Password);
        if (!result.Succeeded)
            return Problem("Password change failed");

        return Ok("Password changed successfully");
    }

    /// <summary>
    /// Send reset link to email
    /// </summary>
    /// <response code="200">Email sent.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Ops! Can't send link.</response>
    [HttpGet]
    public async Task<IActionResult> ResetPassword([FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound("User not found");

        var token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));
        var passwordResetLink = $"https://pifront.netlify.app/account/change/{token}";
        EmailService emailService = new EmailService();

        if (!emailService.SendResetPasswordEmail(email, passwordResetLink))
            return Problem("Unable to send password reset email");

        return Ok("Password reset link sent to email");
    }

    /// <summary>
    /// Confirm email registration
    /// </summary>
    /// <response code="308">Confirmed and redirect to login page.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Ops! Something went wrong.</response>
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound("User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return Problem("Email confirmation failed!");
        
        return Redirect("https://pifront.netlify.app/account/activated");
    }

    #region Private Methods

    private async Task<bool> SendConfirmationEmail(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action("ConfirmEmail", "Authenticate", new { token, email = user.Email }, Request.Scheme);

        EmailService emailService = new EmailService();

        return emailService.SendConfirmEmail(user.Email!, confirmationLink!);
    }

    private async Task<ApplicationUser?> CreateUser(NewUser data, string? editorialOfficeId = null)
    {
        ApplicationUser user = new ApplicationUser
        {
            Email = data.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            Name = data.Name,
            Surname = data.Surname,
            UserName = data.Email,
            EditorialOfficeId = editorialOfficeId
        };

        var result = await _userManager.CreateAsync(user, data.Password);
        return result.Succeeded ? user : null;
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? throw new MissingFieldException("Can't load encode key.")));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    #endregion
}