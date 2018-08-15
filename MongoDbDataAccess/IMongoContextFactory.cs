using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDbAccess
{
    public interface IMongoContextFactory
    {
        /// <summary>
        /// Gets the mongo context.
        /// </summary>
        /// <returns></returns>
        IMongoContext GetMongoContext();

        /// <summary>
        /// Gets the mongo context.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        IMongoContext GetMongoContext(string connectionString);
    }
}
