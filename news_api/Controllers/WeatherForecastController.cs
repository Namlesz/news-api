using Microsoft.AspNetCore.Mvc;
using news_api.Models;
using news_api.Services;

namespace news_api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly NewsService _newsService;

    public WeatherForecastController(NewsService newsService) =>
        _newsService = newsService;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    public async Task<List<Book>> Get() =>
        await _newsService.GetAsync();
}