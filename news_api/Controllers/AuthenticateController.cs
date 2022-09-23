using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user == null)
            return NotFound("User not found");

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized("Email not confirmed");

        if (!await _userManager.CheckPasswordAsync(user, login.Password))
            return Unauthorized("Invalid password");

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
    public async Task<IActionResult> Register([FromBody] User data)
    {
        if (await _userManager.FindByEmailAsync(data.Email) != null)
            return Problem("User already exists!");

        var user = await CreateUser(data);
        if (user == null)
            return Problem("User creation failed! Please check user details and try again.");

        if (!await SendConfirmationEmail(user))
            return Problem("Confirmation email failed to send.");

        return Ok("User created successfully!");
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAdmin([FromBody] User data)
    {
        if (await _userManager.FindByNameAsync(data.Username) != null)
            return Problem("User already exists!");

        var user = await CreateUser(data);
        if (user == null)
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

        return Ok("User created successfully!");
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
        if (user == null)
            return NotFound("User not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResetLink = Url.Action("ChangePassword", "Authenticate", new { token, email }, Request.Scheme);
        EmailHelper emailHelper = new EmailHelper();

        if (passwordResetLink == null || !emailHelper.SendResetPasswordEmail(email, passwordResetLink))
            return Problem("Unable to send password reset email");

        return Ok("Password reset link sent to email");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound("User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return Problem("Email confirmation failed!");

        return Ok("Email confirmed successfully! You can now login.");
    }

    #region Private Methods

    private async Task<bool> SendConfirmationEmail(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = token; // TODO: change to url 
        //Url.Action("ConfirmEmail", "Authenticate", new { token, email = user.Email }, Request.Scheme);

        EmailHelper emailHelper = new EmailHelper();

        return emailHelper.SendConfirmEmail(user.Email, confirmationLink!);
    }

    private async Task<ApplicationUser?> CreateUser(User data)
    {
        ApplicationUser user = new ApplicationUser
        {
            Email = data.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = data.Username,
            Name = data.Name,
            Surname = data.Surname,
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