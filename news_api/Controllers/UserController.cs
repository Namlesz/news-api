using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("{email}")]
    public async Task<IActionResult> Get(string email)
    {
        var user = await _usersCollection.GetUserByEmail(email);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        await _usersCollection.InsertUser(user);
        return CreatedAtAction(nameof(Get), new { email = user.Email }, user);
    }
}