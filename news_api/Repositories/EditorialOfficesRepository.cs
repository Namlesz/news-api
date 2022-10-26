using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Interfaces.Repositories;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class EditorialOfficesRepository : IEditorialOfficesRepository
{
    private readonly IMongoCollection<EditorialOfficeDto> _editorialOffices;

    /// <summary>
    /// Unit test constructor
    /// </summary>
    /// <param name="db">Mock database</param>
    public EditorialOfficesRepository(IMongoCollection<EditorialOfficeDto> db)
    {
        _editorialOffices = db;
    }

    public EditorialOfficesRepository(
        IOptions<NewsDatabaseSettings> databaseOptions,
        IMongoDatabase mongoDatabase)
    {
        _editorialOffices =
            mongoDatabase.GetCollection<EditorialOfficeDto>(databaseOptions.Value.EditorialOfficeCollection);
    }

    public async Task<EditorialOfficeDto?> GetByName(string editorialOfficeName)
        => await _editorialOffices.Find(e => e.Name == editorialOfficeName).FirstOrDefaultAsync();

    public async Task<EditorialOfficeDto?> GetById(Guid id)
        => await _editorialOffices.Find(e => e.Id == id).FirstOrDefaultAsync();
    
    public async void Create(EditorialOfficeDto officeDto)
    {
        await _editorialOffices.InsertOneAsync(officeDto);
    }

    public async void DeleteById(Guid id)
    {
        await _editorialOffices.DeleteOneAsync(e => e.Id == id);
    }
}