using NewsApp.api.Models;

namespace NewsApp.api.Interfaces.Repositories;

public interface IUsersRepository
{
    public Task<UserInfo?> GetUserById(Guid id);
    public Task<List<UserInfo>> GetAllOfficeUsers(string userEditorialOfficeId);
}