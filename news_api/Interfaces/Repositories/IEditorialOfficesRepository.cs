using news_api.Models;

namespace news_api.Interfaces.Repositories;

public interface IEditorialOfficesRepository
{
    public Task<EditorialOffice?> GetByName(string editorialOfficeName);
    public void Create(EditorialOffice office);
    public void DeleteById(Guid id);
    public Task<EditorialOffice?> GetById(Guid id);
}