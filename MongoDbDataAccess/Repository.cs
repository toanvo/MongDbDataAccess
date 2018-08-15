using MongDbDomain;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbDataAccess
{
    public class Repository<T> : IMongoRepository<T> where T : IDomain<ObjectId>
    {
        private readonly IMongoContext _mongoContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRepository{T}"/> class.
        /// </summary>
        /// <param name="mongoContext">The mongo context.</param>
        public Repository(IMongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        public IMongoCollection<T> Collection
        {
            get { return _mongoContext.GetCollection<T>(); }
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Add(T entity)
        {
            AsyncHelpers.RunSync(() => InsertOneAsync(entity));
        }

        /// <summary>
        /// Adds all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public IEnumerable<T> AddAll(IEnumerable<T> entities)
        {
            if (!entities.Any())
            {
                return entities;
            }

            AsyncHelpers.RunSync(() => InsertManyAsync(entities));
            return entities;
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public T Update(T entity)
        {
            AsyncHelpers.RunSync(() => UpdateAsync(entity));
            return entity;
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public IEnumerable<T> UpdateAll(IEnumerable<T> entities)
        {
            if (!entities.Any())
            {
                return entities;
            }

            AsyncHelpers.RunSync(() => ReplaceAllAsync(entities));
            return entities;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(T entity)
        {
            AsyncHelpers.RunSync(() => DeleteAllAsync(e => e.Id == entity.Id));
        }

        /// <summary>
        /// Deletes the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void DeleteById(ObjectId id)
        {
            AsyncHelpers.RunSync(() => DeleteAllAsync(e => e.Id == id));
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        public void DeleteAll()
        {
            AsyncHelpers.RunSync(() => DeleteAllAsync(e => true));
        }

        /// <summary>
        /// Deletes the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Delete(Expression<Func<T, bool>> predicate)
        {
            AsyncHelpers.RunSync(() => DeleteAllAsync(predicate));
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public T GetById(ObjectId id)
        {
            return AsyncHelpers.RunSync(() => FindAsync(x => x.Id == id));
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void DeleteAll(IEnumerable<T> entities)
        {
            if (!entities.Any())
            {
                return;
            }

            AsyncHelpers.RunSync(() => DeleteAllAsync(e => entities.Select(x => x.Id).Contains(e.Id)));
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IList<T> GetAll()
        {
            return AsyncHelpers.RunSync(() => SearchAsync(t => true).ToListAsync());
        }

        /// <summary>
        /// Searches for.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IList<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return AsyncHelpers.RunSync(() => SearchAsync(predicate).ToListAsync());
        }

        /// <summary>
        /// Searches for.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="sortByExpression">The sort by expression.</param>
        /// <returns></returns>
        public IList<T> SearchFor(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sortByExpression, bool isAscending = true)
        {
            return AsyncHelpers.RunSync(() => SearchAsync(predicate, sortByExpression, isAscending).ToListAsync());
        }

        /// <summary>
        /// Searches for paging.
        /// </summary>
        /// <param name="whereExpression">The where expression.</param>
        /// <param name="sortByExpression">The sort by expression.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public IList<T> SearchForPaging(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> sortByExpression, int pageIndex, int pageSize)
        {
            return AsyncHelpers.RunSync(() => SearchForPagingAsync(whereExpression, sortByExpression, pageIndex, pageSize));
        }

        /// <summary>
        /// Finds the one.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public T FindOne(Expression<Func<T, bool>> predicate)
        {
            return AsyncHelpers.RunSync(() => FindAsync(predicate));
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<T>> GetAllAsync()
        {
            return await this.SearchAsync(t => true).ToListAsync();
        }

        /// <summary>
        /// Searches for asynchronous.
        /// </summary>
        /// <param name="whereExpression">The where expression.</param>
        /// <returns></returns>
        public async Task<IList<T>> SearchForAsync(Expression<Func<T, bool>> whereExpression)
        {
            return await this.SearchAsync(whereExpression).ToListAsync();
        }

        /// <summary>
        /// Searches for paging asynchronous.
        /// </summary>
        /// <param name="whereExpression">The where expression.</param>
        /// <param name="sortByExpression">The sort by expression.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task<IList<T>> SearchForPagingAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> sortByExpression, int pageIndex, int pageSize)
        {
            if (whereExpression == null)
            {
                throw new ArgumentNullException(nameof(whereExpression));
            }

            var findExpression = this.SearchAsync(whereExpression);
            if (sortByExpression != null)
            {
                findExpression = findExpression.SortBy(sortByExpression);
            }

            findExpression.Skip(pageIndex * pageSize).Limit(pageSize);

            return await findExpression.ToListAsync();
        }

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <param name="findExpression">The find expression.</param>
        /// <returns></returns>
        public async Task<T> FindAsync(Expression<Func<T, bool>> findExpression)
        {
            return await this.SearchAsync(findExpression).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Inserts the one asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task InsertOneAsync(T entity)
        {
            await Collection.InsertOneAsync(entity);
        }

        /// <summary>
        /// Inserts the many asynchronous.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public async Task InsertManyAsync(IEnumerable<T> entities)
        {
            if (!entities.Any())
            {
                return;
            }

            var requests = new List<WriteModel<T>>();
            requests.AddRange(entities.Select(x => new InsertOneModel<T>(x)));
            await Collection.BulkWriteAsync(requests);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(u => u.Id, entity.Id);
            await Collection.ReplaceOneAsync(filter, entity);
        }

        /// <summary>
        /// Replaces all asynchronous.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public async Task ReplaceAllAsync(IEnumerable<T> entities)
        {
            var writeModels = entities
                .Select(entity => new ReplaceOneModel<T>(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity))
                .Cast<WriteModel<T>>()
                .ToList();

            await Collection.BulkWriteAsync(writeModels);
        }

        /// <summary>
        /// Deletes all asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public async Task DeleteAllAsync(Expression<Func<T, bool>> predicate)
        {
            await Collection.DeleteManyAsync(predicate);
        }

        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <param name="whereExpression">The where expression.</param>
        /// <returns></returns>
        public IFindFluent<T, T> SearchAsync(Expression<Func<T, bool>> whereExpression)
        {
            return this.Collection.Find(Builders<T>.Filter.Where(whereExpression));
        }

        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <param name="whereExpression">The where expression.</param>
        /// <returns></returns>
        public IFindFluent<T, T> SearchAsync(Expression<Func<T, bool>> whereExpression, Expression<Func<T, object>> sortExpression, bool isAscending = true)
        {
            return this.Collection.Find(Builders<T>.Filter.Where(whereExpression)).Sort(isAscending ? Builders<T>.Sort.Ascending(sortExpression) : Builders<T>.Sort.Descending(sortExpression));
        }
    }
}
