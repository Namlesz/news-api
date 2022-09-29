using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using news_api.Models;
using news_api.Repositories;
using news_api.Settings;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class EditorialOfficeController : ControllerBase
{
    private readonly EditorialOfficesRepository _editorialOfficesRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditorialOfficeController(EditorialOfficesRepository editorialOfficesCollection,
        UserManager<ApplicationUser> userManager
    )
    {
        _userManager = userManager;
        _editorialOfficesRepository = editorialOfficesCollection;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] EditorialOffice office)
    {
        if (!office.IsValid())
        {
            return BadRequest("All fields must be filled");
        }

        if (await _editorialOfficesRepository.GetByName(office.Name!) != null)
        {
            return BadRequest("Editorial office already exists");
        }

        if (!await _editorialOfficesRepository.Create(office))
        {
            return Problem("Something went wrong");
        }

        return Ok(office);
    }

    [HttpGet]
    [Authorize]
    [Route("{editorialOfficeName}")]
    public async Task<IActionResult> GetByName([FromRoute] string editorialOfficeName) =>
        Ok(await _editorialOfficesRepository.GetByName(editorialOfficeName));
}