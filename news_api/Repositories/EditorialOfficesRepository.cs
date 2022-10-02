using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class EditorialOfficesRepository
{
    private readonly IMongoCollection<EditorialOffice> _editorialOffices;


    //To Unit tests
    public EditorialOfficesRepository(IMongoCollection<EditorialOffice> db)
    {
        _editorialOffices = db;
    }

    public EditorialOfficesRepository(
        IOptions<NewsDatabaseSettings> databaseOptions,
        IMongoDatabase mongoDatabase)
    {
        _editorialOffices =
            mongoDatabase.GetCollection<EditorialOffice>(databaseOptions.Value.EditorialOfficeCollection);
    }

    public async Task<EditorialOffice?> GetByName(string editorialOfficeName) 
        => await _editorialOffices.Find(e => e.Name == editorialOfficeName).FirstOrDefaultAsync();

    public async void Create(EditorialOffice office)
    {
        await _editorialOffices.InsertOneAsync(office);
    }
    
    public async void DeleteById(Guid id)
    {
        await _editorialOffices.DeleteOneAsync(e => e.Id == id);
    }
    
}