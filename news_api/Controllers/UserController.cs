using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using news_api.Models;
using news_api.Repositories;

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

    [HttpGet("{id:length(36)}")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo(string id)
    {
        var user = await _usersCollection.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}