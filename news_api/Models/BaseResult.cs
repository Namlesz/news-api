namespace news_api.Models;

public class BaseResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}