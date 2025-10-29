using MongoDB.Driver;

namespace HotelLib;

public static class MongoDatabase
{
    private static IMongoDatabase? database;

    public static void Initialize(string databaseName)
    {
#if DEBUG
        const string host = "localhost";
#else
        const string host = "mongo";
#endif

        var client = new MongoClient($"mongodb://{host}:27017");
        database = client.GetDatabase(databaseName);
    }

    public static IMongoCollection<T> GetCollection<T>(string name) => database!.GetCollection<T>(name);
}
