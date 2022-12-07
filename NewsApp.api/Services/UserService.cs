using Microsoft.AspNetCore.Identity;
using NewsApp.api.Context;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Models;

namespace NewsApp.api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
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

    public async Task<IdentityResult> DeleteEditorialOffice(string id)
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

        user.EditorialOfficeId = null;

        return await _userManager.UpdateAsync(user);
    }

    public async Task<bool> HasEditorialOffice(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return false;
        }

        return !string.IsNullOrEmpty(user.EditorialOfficeId);
    }

    public async Task<string> GetUserIdentity(string id)
    {
        var owner = await _userManager.FindByIdAsync(id);
        return $"{owner!.Name} {owner.Surname}";
    }

    public async Task<ApplicationUser?> FindUser(string id) =>
        await _userManager.FindByIdAsync(id);
    
    public async Task<string?> GetEditorialOfficeId(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user?.EditorialOfficeId;
    }
}