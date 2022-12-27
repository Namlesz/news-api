using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using NewsApp.api.Context;
using NewsApp.api.Controllers;
using NewsApp.api.Models;
using NewsApp.api.Repositories;
using NewsApp.api.Services;

namespace NewsApp.Test.tests;

[TestFixture]
public class UserTests
{
    private IMongoDatabase _database = null!;
    private UserController _controller = null!;
    
    private const string IdRedactor = "12345678-1234-1234-1234-123456789123";
    private const string IdEditor = "12345678-1234-1234-1234-123456789124";
    private const string EditorialOfficeId = "87654321-1234-1234-1234-123456789123";
    
    [OneTimeSetUp]
    public void Setup()
    {
        var exampleRedactor = new UserInfo()
        {
            Name = "Adam",
            Email = "adam.kowalski@gmail.com",
            Surname = "Kowalski",
            Id = Guid.Parse(IdRedactor),
            EditorialOfficeId = EditorialOfficeId
        };
        var exampleEditor = new UserInfo()
        {
            Name = "Jan",
            Email = "jan.wojciechowski@gmail.com",
            Surname = "Wojciechowski",
            Id = Guid.Parse(IdEditor),
            EditorialOfficeId = EditorialOfficeId
        };

        _database = MongoDbCreator.CreateDb();
        var collection = _database.GetCollection<UserInfo>("Users");
        collection.InsertOne(exampleRedactor);
        collection.InsertOne(exampleEditor);

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
                        EditorialOfficeId = EditorialOfficeId
                    })!;
                }
                return Task.FromResult(new ApplicationUser())!;
            });
       

        mockUserManager.Setup(
                x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var userLogic = new UserService(mockUserManager.Object);
        var userRepo = new UsersRepository(collection);

        _controller = new UserController(userRepo, userLogic);
    }

    [Test]
    [Description("GetUserInfo() -> Found user")]
    public async Task GetUserInfo()
    {
        var actionResult = await _controller.GetUserInfo(IdRedactor);
        var okResult = actionResult as OkObjectResult;

        var user = okResult?.Value as UserInfo;

        Assert.That(user?.Id.ToString(), Is.EqualTo(IdRedactor));
        Assert.That(user?.Name, Is.EqualTo("Adam"));
        Assert.That(user?.Surname, Is.EqualTo("Kowalski"));
        Assert.That(user?.Email, Is.EqualTo("adam.kowalski@gmail.com"));
    }

    [Test]
    [Description("GetUserInfo() -> Wrong id format")]
    public async Task GetUserInvalidIdFormat()
    {
        var actionResult = await _controller.GetUserInfo("123412");
        var problemResult = actionResult as ObjectResult;

        Assert.That(problemResult?.StatusCode, Is.EqualTo(500));
    }

    [Test]
    [Description("GetUserInfo() -> Not found user")]
    public async Task GetUserWrongId()
    {
        var actionResult = await _controller.GetUserInfo("12345678-1234-1234-1234-123456782223");
        var notFoundResult = actionResult as NotFoundResult;

        Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
    }

    [Test]
    [Description("UpdateUserInfo() -> Updated")]
    public async Task UpdateUserInfo()
    {
        var data = new UserInfo() { Name = "Kowal", Email = "kowal@me.pl" };
        
        var actionResult = await _controller.UpdateUserInfo(IdRedactor, data);
        var okResult = actionResult as OkObjectResult;

        Assert.That(okResult?.Value?.ToString(), Is.EqualTo("User data updated"));
    }

    [Test]
    [Description("GetAllEditorialOfficeUsers() -> Found all users")]
    public async Task GetAllEditorialOfficeUsers()
    {
        var actionResult = await _controller.GetAllOfficeUsers(IdRedactor);
        var okResult = actionResult as OkObjectResult;

        var users = okResult?.Value as List<UserInfo>;

        Assert.That(users!.Count, Is.EqualTo(2));
        Assert.That(users[0].Id, Is.EqualTo(Guid.Parse(IdRedactor)));
        Assert.That(users[1].Id, Is.EqualTo(Guid.Parse(IdEditor)));
    }

    [Test]
    [Description("GetAllEditorialOfficeUsers() -> User don't have editorial office")]
    public async Task NotFoundEditorialOffice()
    {
        var actionResult = await _controller.GetAllOfficeUsers("12345678-4321-1234-1234-123456789123");
        var badRequest = actionResult as BadRequestObjectResult;

        Assert.That(badRequest!.Value, Is.EqualTo("User is not assigned to any editorial office"));
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        MongoDbCreator.Dispose();
    }
}