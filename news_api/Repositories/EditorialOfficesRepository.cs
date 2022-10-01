using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using news_api.Logic;
using news_api.Models;
using news_api.Settings;

namespace news_api.Repositories;

public class EditorialOfficesRepository
{
    private readonly IMongoCollection<EditorialOffice> _editorialOffices;
    private readonly ApplicationUserLogic _applicationUserLogic;


    //To Unit tests
    public EditorialOfficesRepository(IMongoCollection<EditorialOffice> db, ApplicationUserLogic applicationUserLogic)
    {
        _editorialOffices = db;
        _applicationUserLogic = applicationUserLogic;
    }

    public EditorialOfficesRepository(
        IOptions<NewsDatabaseSettings> databaseOptions,
        IMongoDatabase mongoDatabase, UserManager<ApplicationUser> userManager, ApplicationUserLogic applicationUserLogic)
    {
        _applicationUserLogic = applicationUserLogic;
        var dbSettings = databaseOptions.Value;
        _editorialOffices = mongoDatabase.GetCollection<EditorialOffice>(dbSettings.EditorialOfficeCollection);
    }


    public async Task<EditorialOffice?> GetByName(string editorialOfficeName)
    {
        var result = await _editorialOffices.Find(e => e.Name == editorialOfficeName).FirstOrDefaultAsync();
        if(result == null)
        {
            return null;
        }
        
        var owner = await _applicationUserLogic.GetManager().FindByIdAsync(result.OwnerId);
        result.OwnerInfo = $"{owner.Name} {owner.Surname}";
        return result;
    }

    public async Task<bool> Create(EditorialOffice office)
    {
        try
        {
            await _editorialOffices.InsertOneAsync(office);

            var result =
                await _applicationUserLogic.FindAndUpdate(office.OwnerId!,
                    new UserInfo { EditorialOfficeId = office.Id.ToString() });

            if (!result.Succeeded)
                return false;

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}