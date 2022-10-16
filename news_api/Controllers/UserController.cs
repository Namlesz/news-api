using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using news_api.Interfaces.Logic;
using news_api.Interfaces.Repositories;
using news_api.Models;
using news_api.Settings;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IUsersRepository _usersCollection;
    private readonly IApplicationUserLogic _applicationUserLogic;

    public UserController(IUsersRepository usersCollection, IApplicationUserLogic applicationUserLogic)
    {
        _applicationUserLogic = applicationUserLogic;
        _usersCollection = usersCollection;
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAllOfficeUsers([FromQuery] string userId)
    {
        var user = await _applicationUserLogic.GetManager().FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        if (user.EditorialOfficeId is null)
        {
            return BadRequest("User is not assigned to any editorial office");
        }
        
        return Ok(await _usersCollection.GetAllOfficeUsers(user.EditorialOfficeId));
    }

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
        var result = await _applicationUserLogic.FindAndUpdate(id, data);
        
        return !result.Succeeded ? Problem(result.Errors.First().Description) : Ok("User data updated");
    }
}