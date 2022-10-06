using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using news_api.Helpers;
using news_api.Models;
using news_api.Settings;

namespace news_api.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        //TODO: Delete messages from NotFound
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user is null)
            return NotFound("User not found");

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized("Email not confirmed");

        if (!await _userManager.CheckPasswordAsync(user, login.Password))
            return NotFound("Invalid password");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
            expiration = token.ValidTo
        });
    }

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

    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromQuery] string email, [FromQuery] string token,
        [FromQuery] string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound("User not found");

        var result = await _userManager.ResetPasswordAsync(user, token, password);
        if (!result.Succeeded)
            return Problem("Password change failed");

        return Ok("Password changed successfully");
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword([FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound("User not found");

        var token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));
        var passwordResetLink = $"https://pifront.netlify.app/dashboard/change/{token}";
        EmailHelper emailHelper = new EmailHelper();

        if (token is null || !emailHelper.SendResetPasswordEmail(email, passwordResetLink))
            return Problem("Unable to send password reset email");

        return Ok("Password reset link sent to email");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound("User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return Problem("Email confirmation failed!");

        return Redirect("https://pifront.netlify.app/dashboard/activated");
    }

    #region Private Methods

    private async Task<bool> SendConfirmationEmail(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //TODO: Change to production url
        var confirmationLink = Url.Action("ConfirmEmail", "Authenticate", new { token, email = user.Email }, Request.Scheme);

        EmailHelper emailHelper = new EmailHelper();

        return emailHelper.SendConfirmEmail(user.Email, confirmationLink!);
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
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    #endregion
}