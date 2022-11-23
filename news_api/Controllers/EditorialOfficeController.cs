using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using news_api.Interfaces.Logic;
using news_api.Models;
using news_api.Settings;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class EditorialOfficeController : ControllerBase
{
    private readonly IEditorialOfficesLogic _editorialOfficesLogic;

    public EditorialOfficeController(IEditorialOfficesLogic editorialOfficesLogic)
    {
        _editorialOfficesLogic = editorialOfficesLogic;
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
        if (await _editorialOfficesLogic.IsExists(office.Name))
        {
            return BadRequest("Editorial office already exists");
        }

        var result = await _editorialOfficesLogic.Create(office);
        if (!result.Success)
        {
            return Problem(result.Message);
        }

        return Created(nameof(Create), result.Data);
    }

    [HttpGet]
    [Route("{editorialOfficeName}")]
    public async Task<IActionResult> GetByName([FromRoute] string editorialOfficeName)
    {
        var editorialOffice = await _editorialOfficesLogic.GetByName(editorialOfficeName);
        if (editorialOffice is null)
            return NotFound();

        return Ok(editorialOffice);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetById([FromQuery] string id)
    {
        var editorialOffice = await _editorialOfficesLogic.GetById(id);
        if (editorialOffice is null)
            return NotFound();

        return Ok(editorialOffice);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> Delete([FromQuery] string userId, string editorialOfficeId)
    {
        if (!await _editorialOfficesLogic.HasEditorialOffice(userId))
            return NotFound("User doesn't have editorial office");

        var editorialOffice = await _editorialOfficesLogic.GetById(editorialOfficeId);
        if (editorialOffice is null)
            return NotFound("Editorial office not found");
        
        var result = await _editorialOfficesLogic.Delete(userId, editorialOfficeId);

        if (!result.Success)
            return Problem(result.Message);

        return Ok();
    }
}