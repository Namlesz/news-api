using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using news_api.Logic;
using news_api.Models;
using news_api.Repositories;
using news_api.Settings;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly UsersRepository _usersCollection;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UsersRepository usersCollection, UserManager<ApplicationUser> userManager
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
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo([FromQuery] string id, [FromBody] UserInfo data)
    {
        var applicationUserLogic = new ApplicationUserLogic(_userManager);
        var result = await applicationUserLogic.FindAndUpdate(id, data);
        
        return !result.Succeeded ? Problem(result.Errors.First().Description) : Ok("User data updated");
    }
}