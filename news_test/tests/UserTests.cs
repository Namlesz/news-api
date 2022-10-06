using AspNetCore.Identity.MongoDbCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    private IMongoCollection<UserInfo>? _connection;
    private UserController? _controller;

    [OneTimeSetUp]
    public void Setup()
    {
        _connection = MongoDbCreator.CreateConnection<UserInfo>(nameof(UserInfo));
        //IDEA 1
        // var store = new Mock<IUserStore<ApplicationUser>>();
        // store.Setup(x => x.FindByIdAsync("12345678-1234-1234-1234-123456789123", CancellationToken.None))
        //     .ReturnsAsync(new ApplicationUser()
        //     {
        //         Name = "Adam",
        //         Email = "test@email.com",
        //         Surname = "Kowalski",
        //         Id = Guid.Parse("12345678-1234-1234-1234-123456789123")
        //     });
        //
        // var userMng = new UserManager<ApplicationUser>(store.Object, null, null, null, null, null, null, null, null);
        
        //IDEA 2
        // var store = new MongoUserStore(_connection]);
        // var userMng = new Mock<UserManager<ApplicationUser>>(_connection);
        // userMng.Object.CreateAsync(new ApplicationUser()
        // {
        //     Name = "Adam",
        //     Email = "test@email.com",
        //     Surname = "Kowalski",
        //     Id = Guid.Parse("12345678-1234-1234-1234-123456789123")
        // });
        // var userLogic = new ApplicationUserLogic(userMng.Object);

        var userRepo = new UsersRepository(_connection!);
        _controller = new UserController(userRepo, null!);
    }

    [Test]
    [Description("Found a user by id")]
    public async Task GetUserInfo()
    {
        var actionResult = await _controller!.GetUserInfo("12345678-1234-1234-1234-123456789123");

        // Controller result
        var okResult = actionResult as OkObjectResult;
        var user = okResult?.Value as UserInfo;

        //Assert
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

        // Controller result
        var problemResult = actionResult as ObjectResult;

        //Assert
        Assert.That(problemResult?.StatusCode, Is.EqualTo(500));
    }

    [Test]
    [Description("Not found user by id")]
    public async Task GetUserWrongId()
    {
        var actionResult = await _controller!.GetUserInfo("12345678-1234-1234-1234-123456782223");

        // Controller result
        var notFoundResult = actionResult as NotFoundResult;

        //Assert
        Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
    }
    
    [Test]
    [Description("Found a user by id")]
    public async Task UpdateUserInfo()
    {
        var data = new UserInfo() { Name = "Kowal", Email = "kowal@me.pl" };
        var actionResult = await _controller!.UpdateUserInfo("12345678-1234-1234-1234-123456789123", data);

        // Controller result
        var okResult = actionResult as OkObjectResult;

        //Assert
        Assert.That(okResult?.Value?.ToString(), Is.EqualTo("User data updated"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _connection?.DeleteMany(FilterDefinition<UserInfo>.Empty);
        MongoDbCreator.Dispose();
    }
}