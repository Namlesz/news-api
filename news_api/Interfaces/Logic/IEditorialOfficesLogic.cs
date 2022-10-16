using news_api.Models;

namespace news_api.Interfaces.Logic;

public interface IEditorialOfficesLogic
{
    public Task<EditorialOffice?> GetByName(string editorialOfficeName);
    public Task<BaseResult> Create(EditorialOffice office);
}