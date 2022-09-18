using Mongo2Go;
using MongoDB.Driver;

namespace news_test;

public static class MongoDbCreator
{
    private static MongoDbRunner? _runner;

    public static IMongoCollection<T> CreateConnection<T>(string collectionName)
    {
        _runner = MongoDbRunner.Start();
        
        var client = new MongoClient(_runner.ConnectionString);
        var database = client.GetDatabase("test");
        
        return database.GetCollection<T>(collectionName);
    }
    
    public static void Dispose()
    {
        _runner?.Dispose();
    }
}