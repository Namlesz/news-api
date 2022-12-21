using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Interfaces.Repositories;
using NewsApp.api.Models;
using NewsApp.api.Settings;
using Swashbuckle.AspNetCore.Annotations;

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

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation("Get all users from specified office")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<UserInfo>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "User doesn't have an office")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User id not found.")]
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

    [HttpGet]
    [Authorize]
    [SwaggerOperation("Get user info")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(UserInfo))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Invalid user id format.")]
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

    [HttpPut]
    [Authorize]
    [SwaggerOperation("Update user data in database")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ops! Can't update user.")]
    public async Task<IActionResult> UpdateUserInfo([FromQuery] string id, [FromBody] UserInfo data)
    {
        var result = await _userService.FindAndUpdate(id, data);

        return !result.Succeeded ? Problem(result.Errors.First().Description) : Ok("User data updated");
    }
}