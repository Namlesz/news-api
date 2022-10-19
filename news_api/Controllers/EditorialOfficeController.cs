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

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] EditorialOffice office)
    {
        if (!office.IsValid())
        {
            return BadRequest("All fields must be filled");
        }

        if (await _editorialOfficesLogic.IsExists(office.Name!))
        {
            return BadRequest("Editorial office already exists");
        }

        var result = await _editorialOfficesLogic.Create(office);
        if (!result.Success)
        {
            return Problem(result.Message);
        }

        return Created(nameof(Create), office);
    }

    [HttpGet]
    [Authorize]
    [Route("{editorialOfficeName}")]
    public async Task<IActionResult> GetByName([FromRoute] string editorialOfficeName)
    {
        var editorialOffice = await _editorialOfficesLogic.GetByName(editorialOfficeName);
        if (editorialOffice is null)
            return NotFound();

        return Ok(editorialOffice);
    }
}