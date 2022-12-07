namespace NewsApp.api.Models;

public class BaseResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class BaseTypeResult<T> : BaseResult
{
    public T? Data { get; init; }
}