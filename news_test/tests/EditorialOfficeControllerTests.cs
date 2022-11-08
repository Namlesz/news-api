using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using news_api.Controllers;
using news_api.Interfaces.Logic;
using news_api.Logic;
using news_api.Models;
using news_api.Repositories;
using news_api.Settings;

namespace news_test.tests;

public class EditorialOfficeControllerTests
{
    private IMongoDatabase _database = null!;
    private EditorialOfficeController _controller = null!;
    
    private const string IdEditorialOffice = "12345678-1234-1234-1234-123456789321";
    private const string IdRedactor = "12345678-1234-1234-1234-123456789123";

    [OneTimeSetUp]
    public void Setup()
    {
        var exampleEditorialOffice = new EditorialOfficeDto()
        {
            Id = Guid.Parse(IdEditorialOffice),
            Name = "Cracovia News",
            Town = "Cracovia",
            OwnerId = IdRedactor
        };

        _database = MongoDbCreator.CreateDb();
        var collection = _database.GetCollection<EditorialOfficeDto>("EditorialOffices");
        collection.InsertOne(exampleEditorialOffice);
        
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object,
            null, null, null, null, null, null, null, null);

        mockUserManager.Setup(
                manager => manager.FindByIdAsync(It.IsAny<string>()))
            .Returns<string>(x =>
            {
                if (x.Contains(IdRedactor))
                {
                    return Task.FromResult(new ApplicationUser
                    {
                        Name = "Jan",
                        Surname = "Kowalski"
                    });
                }
                return Task.FromResult(new ApplicationUser());
            });
        
        var applicationUserLogic = new Mock<IApplicationUserLogic>();
        applicationUserLogic.Setup(x => x.GetUserIdentity(It.IsAny<string>()))
            .ReturnsAsync("Jan Kowalski");
        
        var editorialOfficeRepository = new EditorialOfficesRepository(collection);
        var editorialOfficeLogic = new EditorialOfficesLogic(editorialOfficeRepository, applicationUserLogic.Object);
        
        _controller = new EditorialOfficeController(editorialOfficeLogic);
    }
    
    [Test]
    [Description("GetByName() -> Get editorial office by name")]
    public async Task GetEditorialOfficeName()
    {
        var actionResult = await _controller.GetByName("Cracovia News");
        var okResult = actionResult as OkObjectResult;

        var office = okResult?.Value as OfficeInfo;

        Assert.That(office?.Name, Is.EqualTo("Cracovia News"));
        Assert.That(office?.Town, Is.EqualTo("Cracovia"));
        Assert.That(office?.OwnerInfo, Is.EqualTo("Jan Kowalski"));
    }
    
    [Test]
    [Description("GetByName() -> Not found editorial office by name")]
    public async Task NotFoundEditorialOfficeName()
    {
        var actionResult = await _controller.GetByName("News Hunter");
        var notFound = actionResult as NotFoundResult;
        
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }
    
    [Test]
    [Description("Create() -> EditorialOffice already exists")]
    public async Task DuplicateEditorialOffice()
    {
        var editorialOffice = new EditorialOfficeDto()
        {
            Name = "Cracovia News",
            Town = "Warszawa",
            OwnerId = "12345678-1234-1234-1234-123456789129"
        };
        
        var actionResult = await _controller.Create(editorialOffice);
        var badResult = actionResult as BadRequestObjectResult;
        
        Assert.That(badResult?.Value, Is.EqualTo("Editorial office already exists"));
    }
    
    [Test]
    [Description("Create() -> InValid model")]
    public async Task InvalidEditorialOfficeModel()
    {
        var editorialOffice = new EditorialOfficeDto()
        {
            OwnerId = "12345678-1234-1234-1234-123456789129"
        };
        
        var actionResult = await _controller.Create(editorialOffice);
        var badResult = actionResult as BadRequestObjectResult;
        
        Assert.That(badResult?.Value, Is.EqualTo("All fields must be filled"));
    }
}