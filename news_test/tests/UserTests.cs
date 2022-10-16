using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using news_api.Controllers;
using news_api.Logic;
using news_api.Models;
using news_api.Repositories;
using news_api.Settings;

namespace news_test.tests;

public class Tests
{
    private IMongoDatabase? _database;
    private UserController? _controller;

    [OneTimeSetUp]
    public void Setup()
    {
        var id = "12345678-1234-1234-1234-123456789123";
        var exampleUser = new UserInfo()
        {
            Name = "Adam",
            Email = "test@email.com",
            Surname = "Kowalski",
            Id = Guid.Parse(id),
            EditorialOfficeId = "87654321-1234-1234-1234-123456789123"
        };

        _database = MongoDbCreator.CreateDb();
        var collection = _database.GetCollection<UserInfo>("Users");
        collection.InsertOne(exampleUser);

        var mockUserManager = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object,
            null, null, null, null, null, null, null, null);

        mockUserManager.Setup(
                manager => manager.FindByIdAsync(It.IsAny<string>()))
            .Returns<string>(x =>
            {
                if (x.Contains(id))
                {
                    return Task.FromResult(new ApplicationUser
                    {
                        EditorialOfficeId = "87654321-1234-1234-1234-123456789123"
                    });
                }

                return Task.FromResult(new ApplicationUser());
            });
       

        mockUserManager.Setup(
                x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var userLogic = new ApplicationUserLogic(mockUserManager.Object);
        var userRepo = new UsersRepository(collection!);

        _controller = new UserController(userRepo, userLogic);
    }

    [Test]
    [Description("Found a user by id")]
    public async Task GetUserInfo()
    {
        var actionResult = await _controller!.GetUserInfo("12345678-1234-1234-1234-123456789123");
        var okResult = actionResult as OkObjectResult;

        var user = okResult?.Value as UserInfo;

        Assert.That(user?.Id.ToString(), Is.EqualTo("12345678-1234-1234-1234-123456789123"));
        Assert.That(user?.Name, Is.EqualTo("Adam"));
        Assert.That(user?.Surname, Is.EqualTo("Kowalski"));
        Assert.That(user?.Email, Is.EqualTo("test@email.com"));
    }

    [Test]
    [Description("Found user wrong id format (not guid)")]
    public async Task GetUserInvalidIdFormat()
    {
        var actionResult = await _controller!.GetUserInfo("123412");
        var problemResult = actionResult as ObjectResult;

        Assert.That(problemResult?.StatusCode, Is.EqualTo(500));
    }

    [Test]
    [Description("Not found user by id")]
    public async Task GetUserWrongId()
    {
        var actionResult = await _controller!.GetUserInfo("12345678-1234-1234-1234-123456782223");
        var notFoundResult = actionResult as NotFoundResult;

        Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
    }

    [Test]
    [Description("Update user information")]
    public async Task UpdateUserInfo()
    {
        var data = new UserInfo() { Name = "Kowal", Email = "kowal@me.pl" };
        
        var actionResult = await _controller!.UpdateUserInfo("12345678-1234-1234-1234-123456789123", data);
        var okResult = actionResult as OkObjectResult;

        Assert.That(okResult?.Value?.ToString(), Is.EqualTo("User data updated"));
    }

    [Test]
    [Description("Get all users from Editorial Office")]
    public async Task GetAllEditorialOfficeUsers()
    {
        var actionResult = await _controller!.GetAllOfficeUsers("12345678-1234-1234-1234-123456789123");
        var okResult = actionResult as OkObjectResult;

        var users = okResult?.Value as List<UserInfo>;

        Assert.That(users!.Count, Is.EqualTo(1));
        Assert.That(users![0].Id, Is.EqualTo(Guid.Parse("12345678-1234-1234-1234-123456789123")));
    }

    [Test]
    [Description("Not found editorial office")]
    public async Task NotFoundEditorialOffice()
    {
        var actionResult = await _controller!.GetAllOfficeUsers("12345678-4321-1234-1234-123456789123");
        var badRequest = actionResult as BadRequestObjectResult;

        Assert.That(badRequest!.Value, Is.EqualTo("User is not assigned to any editorial office"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        MongoDbCreator.Dispose();
    }
}