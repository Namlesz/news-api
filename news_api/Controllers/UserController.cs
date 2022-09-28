using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using news_api.Models;
using news_api.Repositories;
using news_api.Settings;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly UsersRepositories _usersCollection;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UsersRepositories usersCollection, UserManager<ApplicationUser> userManager
    )
    {
        _userManager = userManager;
        _usersCollection = usersCollection;
    }

    //TODO: Get all users by Redaction
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAll() =>
        Ok(await _usersCollection.GetAll());

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserInfo([FromQuery] string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return Problem("Invalid id format");
        }

        var user = await _usersCollection.GetUserById(guid);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo([FromQuery] string id, [FromBody] UserInfo data)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Email = data.Email ?? user.Email;
        user.Name = data.Name ?? user.Name;
        user.Surname = data.Surname ?? user.Surname;
        
        var result = await _userManager.UpdateAsync(user);
        return !result.Succeeded ? Problem(result.Errors.First().Description) : Ok("User data updated");
    }
}