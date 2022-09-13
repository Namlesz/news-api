using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using news_api.Auth;
using news_api.Helpers;
using news_api.Models;
using news_api.Settings;

namespace news_api.Controllers;

[ApiController]
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
    [Route("login")]
    public async Task<IActionResult> Login(Login login)
    {
        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user == null)
            return NotFound("User not found");

        if (! await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized("Email not confirmed");

        if (await _userManager.CheckPasswordAsync(user, login.Password))
        {
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

        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(User model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        
        if (userExists != null)
            return Problem("User already exists!");

        ApplicationUser user = new ApplicationUser
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Name = model.Name,
            Surname =  model.Surname,
        };
        
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return Problem("User creation failed! Please check user details and try again.");
        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = Url.Action("ConfirmEmail", "Authenticate", new { token, email = user.Email }, Request.Scheme);
        EmailHelper emailHelper = new EmailHelper();
        
        bool emailResponse = emailHelper.SendConfirmEmail(user.Email, confirmationLink!);
             
        if (!emailResponse)
        {
            return Problem("Confirmation email failed to send.");
        }

        return Ok("User created successfully!");
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin(User model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return Problem("User already exists!");

        ApplicationUser user = new ApplicationUser
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Name = model.Name,
            Surname =  model.Surname,
        };
        
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return Problem("User creation failed! Please check user details and try again." );

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new ApplicationRole() { Name = UserRoles.Admin });
        
        if (!await _roleManager.RoleExistsAsync(UserRoles.Editor))
            await _roleManager.CreateAsync(new ApplicationRole() { Name = UserRoles.Editor });

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        }

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Editor);
        }

        return Ok("User created successfully!");
    }
    
    [HttpGet]
    [Route("[controller]")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound("User not found");
 
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return Problem("Email confirmation failed!");
        
        return Ok("Email confirmed successfully!");
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
}