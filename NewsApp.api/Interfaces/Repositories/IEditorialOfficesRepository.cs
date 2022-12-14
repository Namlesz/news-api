using MongoDB.Driver;
using NewsApp.api.Models;

namespace NewsApp.api.Interfaces.Repositories;

public interface IEditorialOfficesRepository
{
    public Task<EditorialOfficeDto?> GetByName(string editorialOfficeName);
    public void Create(EditorialOfficeDto officeDto);
    public Task<DeleteResult> DeleteById(Guid id);
    public Task<EditorialOfficeDto?> GetById(Guid id);
}