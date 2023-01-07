using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Models;
using NewsApp.api.Settings;
using Swashbuckle.AspNetCore.Annotations;

namespace NewsApp.api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class EditorialOfficeController : ControllerBase
{
    private readonly IOfficeService _officeService;

    public EditorialOfficeController(IOfficeService officeService)
    {
        _officeService = officeService;
    }

    [HttpPost]
    [SwaggerOperation("Create new editorial office")]
    [SwaggerResponse(StatusCodes.Status201Created, "Office created.")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Editorial office exists.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ops! Can't create office.")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] NewOffice office)
    {
        if (await _officeService.IsExists(office.Name))
        {
            return Conflict("Editorial office already exists");
        }

        var result = await _officeService.Create(office);
        if (!result.Success)
        {
            return Problem(result.Message);
        }

        return Created(nameof(Create), result.Data);
    }

    [HttpGet]
    [Route("{editorialOfficeName}")]
    [SwaggerOperation("Get office info by name")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(OfficeInfo))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found an office.")]
    public async Task<IActionResult> GetByName([FromRoute] string editorialOfficeName)
    {
        var editorialOffice = await _officeService.GetByName(editorialOfficeName);
        if (editorialOffice is null)
            return NotFound();

        return Ok(editorialOffice);
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation("Get office info by name")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(OfficeInfo))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found an office.")]
    public async Task<IActionResult> GetById([FromQuery] string id)
    {
        var editorialOffice = await _officeService.GetById(id);
        if (editorialOffice is null)
            return NotFound();

        return Ok(editorialOffice);
    }

    [HttpDelete]
    [Authorize]
    [SwaggerOperation("Delete office")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Deleted.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not found an office/user don't have office.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Ops! Problem when deleting office.")]
    public async Task<IActionResult> Delete([FromQuery] string userId, string editorialOfficeId)
    {
        if (!await _officeService.HasEditorialOffice(userId))
            return NotFound("User doesn't have editorial office");

        var editorialOffice = await _officeService.GetById(editorialOfficeId);
        if (editorialOffice is null)
            return NotFound("Editorial office not found");

        var result = await _officeService.Delete(userId, editorialOfficeId);

        if (!result.Success)
            return Problem(result.Message);

        return NoContent();
    }
}