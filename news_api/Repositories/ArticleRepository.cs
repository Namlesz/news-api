using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using news_api.Interfaces.Repositories;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly IMongoDatabase _db;
    private readonly string _collectionName;

    public ArticleRepository(
        IOptions<NewsDatabaseSettings> databaseOptions,
        IMongoDatabase mongoDatabase)
    {
        _db = mongoDatabase;
        _collectionName = databaseOptions.Value.ArticleCollection;
    }

    private IMongoCollection<TDocument> GetCollection<TDocument>()
    {
        return _db.GetCollection<TDocument>(_collectionName);
    }

    public async void Create(ArticleWithContent articleDtoDto)
    {
        await GetCollection<ArticleWithContent>().InsertOneAsync(articleDtoDto);
    }

    public async Task<List<ArticleDto>> GetArticles(string officeId, int range, int offset) =>
        await GetCollection<ArticleDto>().AsQueryable()
            .OrderByDescending(x => x.PublishedAt)
            .Where(x => x.OfficeId == officeId)
            .Skip(offset)
            .Take(range)
            .Select(x => x)
            .ToListAsync();

    public async Task<ArticleWithContent?> GetArticle(Guid articleId) => 
        await GetCollection<ArticleWithContent>().AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == articleId);    
}