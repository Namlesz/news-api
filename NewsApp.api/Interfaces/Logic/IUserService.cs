using Microsoft.AspNetCore.Identity;
using NewsApp.api.Context;
using NewsApp.api.Models;

namespace NewsApp.api.Interfaces.Logic;

public interface IUserService
{
    /// <summary>
    /// Updates user information with non-null fields from the model.
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="data">Properties to update</param>
    /// <returns>Identity result</returns>
    public Task<IdentityResult> FindAndUpdate(string id, UserInfo data);

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

    /// <summary>
    /// Find user office.
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>Returns office id if exists else return null</returns>
    public  Task<string?> GetEditorialOfficeId(string id);

}