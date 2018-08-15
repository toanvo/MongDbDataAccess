using MongoDB.Driver;
using System;

namespace MongoDbAccess
{
    public class MongoDbContextFactory : IMongoContextFactory
    {
        private readonly string _connectionString;

        public MongoDbContextFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbContextFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MongoDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets the mongo context.
        /// </summary>
        /// <returns></returns>
        public IMongoContext GetMongoContext()
        {
            var mongoUrlBuilder = new MongoUrlBuilder(_connectionString);
            var mongoClient = new MongoClient(_connectionString);
            return new MongoContext(mongoClient, mongoUrlBuilder.DatabaseName);
        }

        /// <summary>
        /// Gets the mongo context.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public IMongoContext GetMongoContext(string connectionString)
        {
            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var mongoClient = new MongoClient(connectionString);
            return new MongoContext(mongoClient, mongoUrlBuilder.DatabaseName);
        }
    }
}
