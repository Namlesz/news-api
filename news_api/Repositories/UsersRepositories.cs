using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Data;
using news_api.Models;

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
    
    public async Task InsertUser(User user) =>
        await _users.InsertOneAsync(user);
    
    public async Task<User?> GetUserByEmail(string email) =>
        await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
    
}