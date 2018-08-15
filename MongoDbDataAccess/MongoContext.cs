using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace MongoDbAccess
{
    public class MongoContext : IMongoContext
    {
        public IMongoDatabase Database { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoContext"/> class.
        /// </summary>
        /// <param name="mongoClient">The mongo client.</param>
        /// <param name="dbName">Name of the database.</param>
        public MongoContext(IMongoClient mongoClient, string dbName)
        {
            var pack = new ConventionPack()
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("CamelCaseConvensions", pack, t => true);
            Database = mongoClient.GetDatabase(dbName);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection<T>()
        {
            return this.Database.GetCollection<T>(typeof(T).Name);
        }
    }
}
