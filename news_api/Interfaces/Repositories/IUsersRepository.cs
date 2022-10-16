using news_api.Models;

namespace news_api.Interfaces.Repositories;

public interface IUsersRepository
{
    public Task<UserInfo?> GetUserById(Guid id);
    public Task<List<UserInfo>> GetAllOfficeUsers(string userEditorialOfficeId);
}