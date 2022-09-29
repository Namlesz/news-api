namespace news_api.Settings;

public class NewsDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UserCollection { get; set; } = null!;
    public string EditorialOfficeCollection { get; set; } = null!;
}