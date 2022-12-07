namespace NewsApp.api.Context;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UserCollection { get; set; } = null!;
    public string EditorialOfficeCollection { get; set; } = null!;
    public string ArticleCollection { get; set; } = null!;
}