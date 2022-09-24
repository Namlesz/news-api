using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class UsersRepositories
{
    private readonly IMongoCollection<NewUser> _users;

    //To Unit tests
    public UsersRepositories(IMongoCollection<NewUser> db)
    {
        _users = db;
    }

    public UsersRepositories(
        IOptions<NewsDatabaseSettings> databaseOptions,
        IMongoDatabase mongoDatabase)
    {
        var dbSettings = databaseOptions.Value;
        _users = mongoDatabase.GetCollection<NewUser>(dbSettings.UserCollection);
    }

    public async Task<List<NewUser>> GetAll() =>
        await _users.Find(user => true).ToListAsync();

    public async Task<NewUser?> GetUserByEmail(string email) =>
        await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

    public async Task<NewUser?> GetUserById(Guid id) =>
        await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
    
}