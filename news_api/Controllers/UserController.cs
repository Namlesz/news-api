using Microsoft.AspNetCore.Authorization;
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

    public UserController(UsersRepositories usersCollection)
    {
        _usersCollection = usersCollection;
    }

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
    public async Task<IActionResult> UpdateUserInfo([FromQuery] string id, [FromBody] User data)
    {
        var d = data;
        if (!Guid.TryParse(id, out var guid))
        {
            return Problem("Invalid id format");
        }

        var user = await _usersCollection.GetUserById(guid);
        if (user == null)
        {
            return NotFound();
        }

        return Ok();
    }
}