using news_api.Models;

namespace news_api.Interfaces.Logic;

public interface IArticleLogic
{
    /// <summary>
    /// Validate file type
    /// </summary>
    /// <param name="file">IFormFile to check</param>
    /// <returns></returns>
    public bool IsAcceptedType(IFormFile file);

    /// <summary>
    /// Add article to database
    /// </summary>
    /// <param name="data">NewArticle model</param>
    /// <returns>BaseResult with success flag and specific message according to result</returns>
    Task<BaseResult> AddArticle(NewArticle data);

    /// <summary>
    /// Get all articles from office by offset and limit
    /// </summary>
    /// <param name="officeId">Editorial office id</param>
    /// <param name="range">Size of articles to get</param>
    /// <param name="offset">Start index where index 0 is the newest article</param>
    /// <returns>List of office articles</returns>
    Task<List<Article>> GetArticles(string officeId, int range, int offset);
}