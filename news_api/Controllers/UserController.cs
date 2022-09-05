using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using news_api.Auth;
using news_api.Models;
using news_api.Repositories;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UsersRepositories _usersCollection;

    public UserController(UsersRepositories usersCollection)
    {
        _usersCollection = usersCollection;
    }
    
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _usersCollection.GetAll();
        return Ok(users);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await _usersCollection.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}