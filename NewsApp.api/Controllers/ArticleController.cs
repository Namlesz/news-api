using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Models;
using Swashbuckle.AspNetCore.Annotations;

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

    [HttpPost]
    [Authorize]
    [SwaggerOperation("Save article to database")]
    [SwaggerResponse(StatusCodes.Status200OK, "Article created.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Wrong image type.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Error, can't create article.")]
    public async Task<IActionResult> CreateArticle([FromForm] NewArticle article)
    {
        if (!_articleService.IsAcceptedImageType(article.Image))
        {
            return BadRequest("Invalid image type");
        }

        var result = await _articleService.AddArticle(article);
        if (!result.Success)
        {
            return Problem(result.Message);
        }

        return Ok(new { id = result.Data!.Id });
    }

    [HttpPut]
    [Authorize]
    [SwaggerOperation("Save article to database")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Article updated.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Can't update")]
    public async Task<IActionResult> UpdateContent([FromBody] ArticleContent articleContent)
    {
        var result = await _articleService.UpdateContent(articleContent.Id, articleContent.Content);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return NoContent();
    }

    [HttpGet]
    [SwaggerOperation("Get all articles by office id")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<Article>))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
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

    [HttpGet]
    [SwaggerOperation("Return article with content")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(ArticleWithContent))]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
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
    [SwaggerOperation("Get article Thumbnail image")]
    [SwaggerResponse(StatusCodes.Status200OK, "Return image in base64.")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetThumbnail([FromQuery] string articleId)
    {
        var result = await _articleService.GetArticleThumbnail(articleId);
        if (result is null)
        {
            return NotFound();
        }

        return Ok(new { Thumbnail = result });
    }

    [HttpDelete]
    [Authorize]
    [SwaggerOperation("Deletes article from db")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Article deleted.")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteArticle([FromQuery] string articleId)
    {
        var result = await _articleService.DeleteArticle(articleId);
        if (!result.Success)
        {
            return NotFound(result.Message);
        }

        return NoContent();
    }

    [HttpGet]
    [SwaggerOperation("Get number of articles in office")]
    [SwaggerResponse(StatusCodes.Status200OK, "Return Count:int")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticleCounts(string officeId)
    {
        var result = await _articleService.GetArticleCounts(officeId);
        if (result is 0)
        {
            return NotFound();
        }

        return Ok(new { Count = result });
    }
}