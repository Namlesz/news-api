using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Interfaces.Repositories;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly IMongoCollection<UserInfo> _users;

    /// <summary>
    /// Unit test constructor
    /// </summary>
    /// <param name="db">Mock db</param>
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

    public async Task<UserInfo?> GetUserById(Guid id) =>
        await _users.Find(user => user.Id == id).FirstOrDefaultAsync();

    public async Task<List<UserInfo>> GetAllOfficeUsers(string userEditorialOfficeId) =>
        await _users.Find(user => user.EditorialOfficeId == userEditorialOfficeId).ToListAsync();
}