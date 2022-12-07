using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NewsApp.api.Context;
using NewsApp.api.Interfaces.Repositories;
using NewsApp.api.Models;

namespace NewsApp.api.Repositories;

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
        IOptions<DatabaseSettings> databaseOptions,
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