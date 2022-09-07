using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class UsersRepositories
{
    private readonly IMongoCollection<User> _users;
    
    public UsersRepositories(
        IOptions<NewsDatabaseSettings> databaseOptions,
        IMongoDatabase mongoDatabase)
    {
        var dbSettings = databaseOptions.Value;
        _users = mongoDatabase.GetCollection<User>(dbSettings.UserCollection);
    }
    
    public async Task<List<User>> GetAll()
    {
        return await _users.Find(user => true).ToListAsync();
    }

    public async Task InsertUser(User user) =>
        await _users.InsertOneAsync(user);
    
    public async Task<User?> GetUserByEmail(string email) =>
        await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
    
    public async Task<User?> GetUserById(string id) =>
        await _users.Find(user => user.Id == Guid.Parse(id)).FirstOrDefaultAsync();

}