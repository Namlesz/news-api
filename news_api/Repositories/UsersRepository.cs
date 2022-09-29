using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class UsersRepository
{
    private readonly IMongoCollection<UserInfo> _users;

    //To Unit tests
    public UsersRepository(IMongoCollection<UserInfo> db)
    {
        _users = db;
    }

    public UsersRepository(
        IOptions<NewsDatabaseSettings> databaseOptions,
        IMongoDatabase mongoDatabase)
    {
        var dbSettings = databaseOptions.Value;
        _users = mongoDatabase.GetCollection<UserInfo>(dbSettings.UserCollection);
    }

    public async Task<List<UserInfo>> GetAll() =>
        await _users.Find(user => true).ToListAsync();

    public async Task<UserInfo?> GetUserByEmail(string email) =>
        await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

    public async Task<UserInfo?> GetUserById(Guid id) =>
        await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
}