using Microsoft.AspNetCore.Identity;
using news_api.Models;
using news_api.Settings;

namespace news_api.Interfaces.Logic;

public interface IApplicationUserLogic
{
    /// <summary>
    /// Updates user information with non-null fields from the model.
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="data">Properties to update</param>
    /// <returns>Identity result</returns>
    public Task<IdentityResult> FindAndUpdate(string id, UserInfo data);

    //TODO: Delete in next update
    //public UserManager<ApplicationUser> GetManager();

    /// <summary>
    /// Check if user has assigned editorial office.
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>True if user have editorial office, false if not.</returns>
    public Task<bool> HasEditorialOffice(string id);

    /// <summary>
    /// Unset user's editorial office.
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>IdentityResult (success if updated) </returns>
    public Task<IdentityResult> DeleteEditorialOffice(string id);

    /// <summary>
    /// Gets user name and surname.
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>Concatenated name and surname with space between</returns>
    public Task<string> GetUserIdentity(string id);

    /// <summary>
    /// Wrapper for UserManager.FindByIdAsync
    /// </summary>
    /// <param name="id">The user ID to search for.</param>
    /// <returns>The Task that represents the asynchronous operation, containing the user matching the specified userId if it exists.</returns>
    public Task<ApplicationUser?> FindUser(string id);
}