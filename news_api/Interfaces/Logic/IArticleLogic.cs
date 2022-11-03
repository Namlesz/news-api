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
}