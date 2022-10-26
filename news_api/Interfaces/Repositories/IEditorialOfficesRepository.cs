using news_api.Models;

namespace news_api.Interfaces.Repositories;

public interface IEditorialOfficesRepository
{
    public Task<EditorialOfficeDto?> GetByName(string editorialOfficeName);
    public void Create(EditorialOfficeDto officeDto);
    public void DeleteById(Guid id);
    public Task<EditorialOfficeDto?> GetById(Guid id);
}