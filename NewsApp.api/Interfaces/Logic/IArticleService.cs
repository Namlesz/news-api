using NewsApp.api.Models;

namespace NewsApp.api.Interfaces.Logic;

public interface IArticleService
{
    /// <summary>
    /// Validate content type
    /// </summary>
    /// <param name="file">IFormFile to check</param>
    /// <returns></returns>
    public bool IsAcceptedContentType(IFormFile file);
    
    /// <summary>
    /// Validate image type
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public bool IsAcceptedImageType(IFormFile file);

    /// <summary>
    /// Add article to database
    /// </summary>
    /// <param name="data">NewArticle model</param>
    /// <returns>BaseResult with success flag and specific message according to result</returns>
    public Task<BaseTypeResult<Article>> AddArticle(NewArticle data);

    /// <summary>
    /// Get all articles from office by offset and limit
    /// </summary>
    /// <param name="officeId">Editorial office id</param>
    /// <param name="range">Size of articles to get</param>
    /// <param name="offset">Start index where index 0 is the newest article</param>
    /// <returns>List of office articles</returns>
    public Task<List<Article>> GetArticles(string officeId, int range, int offset);

    /// <summary>
    /// Find article by id
    /// </summary>
    /// <param name="articleId">Article id</param>
    /// <returns>Article with content</returns>
    public Task<ArticleWithContent?> GetArticle(string articleId);

    /// <summary>
    /// Update article contents
    /// </summary>
    /// <param name="id">Article id</param>
    /// <param name="content">Updated content</param>
    /// <returns>BaseResult with success true if ok or false with message when error </returns>
    public Task<BaseResult> UpdateContent(string id, string content);

    /// <summary>
    /// Gets article thumbnail from database in base64 format
    /// </summary>
    /// <param name="id">id of article</param>
    /// <returns>Base64 string</returns>
    public Task<string?> GetArticleThumbnail(string id);
    
    /// <summary>
    /// Deletes article from database
    /// </summary>
    /// <param name="articleId">id of article</param>
    /// <returns>Flag if article is deleted or not</returns>
    public Task<BaseResult> DeleteArticle(string articleId);
}