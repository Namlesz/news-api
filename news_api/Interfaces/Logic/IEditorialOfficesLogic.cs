using news_api.Models;

namespace news_api.Interfaces.Logic;

public interface IEditorialOfficesLogic
{
    /// <summary>
    /// Get basic information about editorial offices by name.
    /// </summary>
    /// <param name="editorialOfficeName">Name of editorial office</param>
    /// <returns>EditorialOffice</returns>
    public Task<OfficeInfo?> GetByName(string editorialOfficeName);
    
    /// <summary>
    /// Get basic information about editorial offices by id.
    /// </summary>
    /// <param name="id">Editorial office id</param>
    /// <returns>EditorialOffice</returns>
    public Task<OfficeInfo?> GetById(string id);
    
    /// <summary>
    /// Create new editorial office and attach it to user.
    /// </summary>
    /// <param name="officeDto">Editorial office data</param>
    /// <returns>BaseResult with Success flag</returns>
    /// <exception cref="InvalidOperationException">If an error occurs, the editorial office is removed from the database.</exception>
    public Task<BaseTypeResult<EditorialOfficeDto>> Create(NewOffice officeDto);
    
    /// <summary>
    /// Check if editorial office with given name exists.
    /// </summary>
    /// <param name="editorialOfficeName">Editorial office name</param>
    /// <returns>True if office exists</returns>
    public Task<bool> IsExists(string editorialOfficeName);
    
    /// <summary>
    /// Check if user is assigned to editorial office.
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>True if user is assigned to any office</returns>
    public Task<bool> HasEditorialOffice(string id);
    
    
    /// <summary>
    /// Delete editorial office from user and database.
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="editorialOfficeId">Editorial office id</param>
    /// <returns>BaseResult with Success flag.</returns>
    public Task<BaseResult> Delete(string userId, string editorialOfficeId);
}