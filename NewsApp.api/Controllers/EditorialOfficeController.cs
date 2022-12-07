using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Models;
using NewsApp.api.Settings;

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

    /// <summary>
    /// Create new editorial office
    /// </summary>
    /// <response code="201">Office created.</response>
    /// <response code="400">Editorial office exists.</response>
    /// <response code="500">Ops! Can't create office.</response>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] NewOffice office)
    {
        if (await _officeService.IsExists(office.Name))
        {
            return BadRequest("Editorial office already exists");
        }

        var result = await _officeService.Create(office);
        if (!result.Success)
        {
            return Problem(result.Message);
        }

        return Created(nameof(Create), result.Data);
    }

    /// <summary>
    /// Get office info by name
    /// </summary>
    /// <response code="200">Return office.</response>
    /// <response code="404">Not found an office.</response>
    [HttpGet]
    [Route("{editorialOfficeName}")]
    public async Task<IActionResult> GetByName([FromRoute] string editorialOfficeName)
    {
        var editorialOffice = await _officeService.GetByName(editorialOfficeName);
        if (editorialOffice is null)
            return NotFound();

        return Ok(editorialOffice);
    }

    /// <summary>
    /// Get office info by name
    /// </summary>
    /// <response code="200">Return office.</response>
    /// <response code="404">Not found an office.</response>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetById([FromQuery] string id)
    {
        var editorialOffice = await _officeService.GetById(id);
        if (editorialOffice is null)
            return NotFound();

        return Ok(editorialOffice);
    }

    /// <summary>
    /// Get office info by name
    /// </summary>
    /// <response code="200">Deleted.</response>
    /// <response code="404">Not found an office/user don't have office.</response>
    /// <response code="500">Ops! Problem when deleting office.</response>
    [HttpDelete]
    [Authorize]
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

        return Ok();
    }
}