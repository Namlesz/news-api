using news_api.Models;

namespace news_api.Interfaces.Repositories;

public interface IArticleRepository
{
    /// <summary>
    /// Add a new article to the database
    /// </summary>
    /// <param name="articleDtoDto">Article object</param>
    public void Create(ArticleWithContent articleDtoDto);
    
    /// <summary>
    /// Get all articles from the database
    /// </summary>
    /// <param name="officeId">Editorial office id</param>
    /// <param name="range">Size of elements to take</param>
    /// <param name="offset">Starting index</param>
    /// <returns>Returns Article List or empty list</returns>
    public Task<List<ArticleDto>> GetArticles(string officeId, int range, int offset);

}