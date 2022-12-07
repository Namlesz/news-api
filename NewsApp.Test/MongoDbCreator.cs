using Mongo2Go;
using MongoDB.Driver;

namespace NewsApp.Test;

public static class MongoDbCreator
{
    private static MongoDbRunner? _runner;
    private static MongoClient? _client;
    
    public static IMongoDatabase CreateDb()
    {
        _runner = MongoDbRunner.Start();
        _client = new MongoClient(_runner.ConnectionString);
        return _client.GetDatabase("test");
    }
    
    public static void Dispose()
    {
        _client?.DropDatabase("test");
        _runner?.Dispose();
    }
}