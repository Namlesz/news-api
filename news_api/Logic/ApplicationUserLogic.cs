using Microsoft.AspNetCore.Identity;
using news_api.Models;
using news_api.Settings;

namespace news_api.Logic;

public class ApplicationUserLogic
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationUserLogic(UserManager<ApplicationUser> userManager
    )
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> FindAndUpdate(string id, UserInfo data)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "UserNotFound",
                Description = "User not found"
            });
        }

        user.Email = data.Email ?? user.Email;
        user.Name = data.Name ?? user.Name;
        user.Surname = data.Surname ?? user.Surname;
        user.EditorialOfficeId = data.EditorialOfficeId ?? user.EditorialOfficeId;
        
       return await _userManager.UpdateAsync(user);
    }
}