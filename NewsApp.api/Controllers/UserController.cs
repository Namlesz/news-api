using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Interfaces.Repositories;
using NewsApp.api.Models;
using NewsApp.api.Settings;

namespace NewsApp.api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IUsersRepository _usersCollection;
    private readonly IUserService _userService;

    public UserController(IUsersRepository usersCollection, IUserService userService)
    {
        _userService = userService;
        _usersCollection = usersCollection;
    }

    /// <summary>
    /// Get all users from specified office
    /// </summary>
    /// <response code="200">Return list of users.</response>
    /// <response code="400">User doesn't have an office.</response>
    /// <response code="404">User id not found.</response>
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> GetAllOfficeUsers([FromQuery] string userId)
    {
        var user = await _userService.FindUser(userId);
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
    
    /// <summary>
    /// Get all users from specified office
    /// </summary>
    /// <response code="200">Return user info.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Invalid user id format .</response>
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

    /// <summary>
    /// Update user data in database
    /// </summary>
    /// <response code="200">Updated.</response>
    /// <response code="500">Ops! Can't update user.</response>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo([FromQuery] string id, [FromBody] UserInfo data)
    {
        var result = await _userService.FindAndUpdate(id, data);

        return !result.Succeeded ? Problem(result.Errors.First().Description) : Ok("User data updated");
    }
}