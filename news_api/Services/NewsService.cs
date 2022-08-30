using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Data;
using news_api.Models;

namespace news_api.Services;

public class NewsService
{
    private readonly IMongoCollection<Book> _userCollection;

    public NewsService(
        IOptions<NewsDatabaseSettings> newsDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            newsDatabaseSettings.Value.ConnectionString);
        
        var mongoDatabase = mongoClient.GetDatabase(
            newsDatabaseSettings.Value.DatabaseName);

        _userCollection = mongoDatabase.GetCollection<Book>(
            newsDatabaseSettings.Value.CollectionName);
    }

    public async Task<List<Book>> GetAsync() =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<Book?> GetAsync(string id) =>
        await _userCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Book newBook) =>
        await _userCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _userCollection.DeleteOneAsync(x => x.Id == id);
}