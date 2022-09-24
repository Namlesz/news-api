using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using news_api.Controllers;
using news_api.Models;
using news_api.Repositories;

namespace news_test.tests;

public class Tests
{
    private IMongoCollection<NewUser>? _connection;
    private UserController? _controller;

    [OneTimeSetUp]
    public void Setup()
    {
        _connection = MongoDbCreator.CreateConnection<NewUser>(nameof(NewUser));
        _connection?.InsertOne(new NewUser
        {
            Id = Guid.Parse("12345678-1234-1234-1234-123456789123"),
            Name = "Adam",
            Email = "namlesz@gmail.com",
            Surname = "Wisniewski",
            Username = "Namlesz",
        });
        
        var userRepo = new UsersRepositories(_connection!);
        _controller = new UserController(userRepo, null);
    }

    [Test]
    [Description("Found a user by id")]
    public async Task GetUserInfo()
    {
        var actionResult = await _controller!.GetUserInfo("12345678-1234-1234-1234-123456789123");

        // Controller result
        var okResult = actionResult as OkObjectResult;
        var user = okResult?.Value as NewUser;
        
        //Assert
        Assert.That(user?.Id.ToString(), Is.EqualTo("12345678-1234-1234-1234-123456789123"));
        Assert.That(user?.Name, Is.EqualTo("Adam"));
        Assert.That(user?.Surname, Is.EqualTo("Wisniewski"));
        Assert.That(user?.Email, Is.EqualTo("namlesz@gmail.com"));
        Assert.That(user?.Username, Is.EqualTo("Namlesz"));
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
    
    [OneTimeTearDown]
    public void TearDown()
    {
        _connection?.DeleteMany(FilterDefinition<NewUser>.Empty);
        MongoDbCreator.Dispose();
    }
    
}