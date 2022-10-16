using Microsoft.AspNetCore.Identity;
using news_api.Models;
using news_api.Settings;

namespace news_api.Interfaces.Logic;

public interface IApplicationUserLogic
{
    public Task<IdentityResult> FindAndUpdate(string id, UserInfo data);
    public UserManager<ApplicationUser> GetManager();
    public Task<bool> HasEditorialOffice(string id);

}