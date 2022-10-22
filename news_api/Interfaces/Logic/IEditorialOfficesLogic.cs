using news_api.Models;

namespace news_api.Interfaces.Logic;

public interface IEditorialOfficesLogic
{
    public Task<EditorialOffice?> GetByName(string editorialOfficeName);
    public Task<EditorialOffice?> GetById(string id);
    public Task<BaseResult> Create(EditorialOffice office);
    public Task<bool> IsExists(string editorialOfficeName);
    public Task<bool> HasEditorialOffice(string id);
    public Task<BaseResult> Delete(string userId, string editorialOfficeId);
}