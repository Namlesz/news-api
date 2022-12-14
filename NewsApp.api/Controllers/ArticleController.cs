using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Models;

namespace NewsApp.api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// Save article to database
    /// </summary>
    /// <response code="200">Article created.</response>
    /// <response code="400">Wrong content/image type.</response>
    /// <response code="500">Ops! Can't create article.</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateArticle([FromForm] NewArticle article)
    {
        if (!_articleService.IsAcceptedImageType(article.Image))
        {
            return BadRequest("Invalid image type");
        }

        var result = await _articleService.AddArticle(article);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(new { id = result.Data!.Id });
    }

    /// <summary>
    /// Add/update content to article
    /// </summary>
    /// <response code="204">Article created.</response>
    /// <response code="400">Error message in details</response>
    /// <response code="500">Ops! Can't update article.</response>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateContent([FromBody] ArticleContent articleContent)
    {
        var result = await _articleService.UpdateContent(articleContent.Id, articleContent.Content);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return NoContent();
    }

    //TODO: Add optional filters
    /// <summary>
    /// Get all articles from office
    /// </summary>
    /// <response code="200">Return list of articles.</response>
    /// <response code="404">Not found an article.</response>
    [HttpGet]
    public async Task<IActionResult> GetArticles([FromQuery] string officeId, [FromQuery] int range = 10,
        [FromQuery] int offset = 0)
    {
        var result = await _articleService.GetArticles(officeId, range, offset);
        if (result.Count <= 0)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get article details (including content)
    /// </summary>
    /// <response code="200">Return article.</response>
    /// <response code="404">Not found article.</response>
    [HttpGet]
    public async Task<IActionResult> GetArticle([FromQuery] string articleId)
    {
        var result = await _articleService.GetArticle(articleId);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetThumbnail([FromQuery] string articleId)
    {
        var result = await _articleService.GetArticleThumbnail(articleId);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(new { Thumbnail = result });
    }
}