using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using news_api.Interfaces.Logic;
using news_api.Models;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleLogic _articleLogic;

    public ArticleController(IArticleLogic articleLogic)
    {
        _articleLogic = articleLogic;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateArticle([FromForm] NewArticle article)
    {
        if (!_articleLogic.IsAcceptedContentType(article.Content))
        {
            return BadRequest("Invalid content type");
        }

        if (!_articleLogic.IsAcceptedImageType(article.Image))
        {
            return BadRequest("Invalid image type");
        }

        var result = await _articleLogic.AddArticle(article);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    //TODO: Add optional filters 
    [HttpGet]
    public async Task<IActionResult> GetArticles([FromQuery] string officeId, [FromQuery] int range = 10,
        [FromQuery] int offset = 0)
    {
        var result = await _articleLogic.GetArticles(officeId, range, offset);
        if (result.Count <= 0)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetArticle([FromQuery] string articleId)
    {
        var result = await _articleLogic.GetArticle(articleId);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}