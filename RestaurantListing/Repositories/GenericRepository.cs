using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RestaurantListing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantListing.Repositories
{
    public class GenericRepository<T> : 
        IGenericRepository<T> where T : class
    {
        // inject database context in ctor
        private readonly DatabaseContext _databaseContext;

        private DbSet<T> _db;

        public GenericRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            // set a specific db entity
            _db = _databaseContext.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public Task<T> Get(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<T>> GetAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _db;
            if(expression != null)
                query = query.Where(expression);
            if(include != null)
                query = include(query);
            if(orderBy != null)
                query=orderBy(query);

            return await query.AsNoTracking().ToListAsync();

        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity);
            _databaseContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
