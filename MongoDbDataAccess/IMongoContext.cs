using MongoDB.Driver;

namespace MongoDbAccess
{
    public interface IMongoContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<T> GetCollection<T>();
    }
}
