using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

    public async void Create(ArticleWithContent articleDto)
    {
        await GetCollection<ArticleWithContent>().InsertOneAsync(articleDto);
    } 
}