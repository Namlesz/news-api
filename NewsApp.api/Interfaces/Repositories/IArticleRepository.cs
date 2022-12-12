using MongoDB.Driver;
using NewsApp.api.Models;

namespace NewsApp.api.Interfaces.Repositories;

public interface IArticleRepository
{
    /// <summary>
    /// Add a new article to the database
    /// </summary>
    /// <param name="articleDtoDto">Article object</param>
    public void Create(Article articleDtoDto);
    
    /// <summary>
    /// Get all articles from the database
    /// </summary>
    /// <param name="officeId">Editorial office id</param>
    /// <param name="range">Size of elements to take</param>
    /// <param name="offset">Starting index</param>
    /// <returns>Returns Article List or empty list</returns>
    public Task<List<ArticleDto>> GetArticles(string officeId, int range, int offset);

    /// <summary>
    /// Get an article by id
    /// </summary>
    /// <param name="articleId">Article id (GUID)</param>
    /// <returns>Article with content</returns>
    public Task<ArticleWithContent?> GetArticle(Guid articleId);
    
    /// <summary>
    /// Updates an article in db
    /// </summary>
    /// <param name="article">article to update (with id)</param>
    /// <returns>Replacement result</returns>
    public Task<ReplaceOneResult> Update(ArticleWithContent article);
    
    public Task<ArticleThumbnail?> GetThumbnail(Guid articleId);
}