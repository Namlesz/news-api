using news_api.Models;

namespace news_api.Interfaces.Repositories;

public interface IArticleRepository
{
    /// <summary>
    /// Add a new article to the database
    /// </summary>
    /// <param name="articleDto">Article object</param>
    public void Create(ArticleWithContent articleDto);

}