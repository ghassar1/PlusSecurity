using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AcSys.Core.Data.Model.Base;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;
using AcSys.Core.Extensions;

namespace AcSys.Core.Data.Repository
{
    public class GenericRepository<TDbContext, T> : IGenericRepository<T>, IDisposable
        where T : EntityBase
        where TDbContext : DbContext
    {
        //protected DbContext _context;
        //DbContext Context { get; set; }
        TDbContext Context { get; set; }

        //protected readonly DbSet<TEntity> _dbSet;
        public DbSet<T> DbSet { get; set; }

        public GenericRepository(TDbContext context)
        {
            if (context == null)
                throw new ApplicationException("DbContext must be provided.");

            Context = context;
            DbSet = Context.Set<T>();
        }

        public void Dispose()
        {
            //if (Context != null)
            //{
            //    Context.Dispose();
            //    Context = null;
            //}
        }

        public IQueryable<T> GetAsQueryable()
        {
            return DbSet.AsQueryable();
        }

        #region IGenericRepository Query Specification Members

        public async Task<bool> Any(ISearchQuery<T> query)
        {
            return await Any(query.ToSpec());
        }

        public async Task<bool> Any(ISpecification<T> spec)
        {
            var query = GetQuery(spec.FetchStrategy);
            return await query.AnyAsync(spec.Predicate);
        }

        public async Task<T> FirstOrDefault(ISearchQuery<T> query)
        {
            return await FirstOrDefault(query.ToSpec());
        }

        public async Task<T> FirstOrDefault(ISpecification<T> spec)
        {
            var query = GetQuery(spec.FetchStrategy);
            return await query.FirstOrDefaultAsync(spec.Predicate);
        }

        public async Task<ISearchResult<T>> Find(ISearchQuery<T> query)
        {
            ISearchResult<T> results = await Find(query.ToSpec());
            results.Query = query;
            results.PageNo = query.PageNo;
            results.PageSize = query.PageSize;
            return results;
        }

        public async Task<ISearchResult<T>> Find(ISpecification<T> spec)
        {
            IQueryable<T> query = GetQuery(spec.FetchStrategy);
            ISearchResult<T> results = await spec.SatisfyingItemsFrom(query);
            return results;
        }

        public async Task<int> Count(ISearchQuery<T> query)
        {
            int results = await Count(query.ToSpec());
            return results;
        }

        public async Task<int> Count(ISpecification<T> spec)
        {
            IQueryable<T> query = GetQuery(spec.FetchStrategy);
            int results = await spec.Count(query);
            return results;
        }

        #endregion

        private IQueryable<T> GetQuery(IFetchStrategy<T> fetchStrategy)
        {
            //ObjectQuery<T> query = Context.CreateObjectSet<T>();
            IQueryable<T> query = GetAsQueryable();

            if (fetchStrategy == null || fetchStrategy.IncludePaths == null || fetchStrategy.IncludePaths.Count() == 0)
            {
                return query;
            }

            foreach (var path in fetchStrategy.IncludePaths)
            {
                query = query.Include(path);
            }

            return query;
        }

        //public async Task<TOrg> FindAsync<TOrg>(Guid orgId, Guid id, bool throwIfNotFound = false) where TOrg : IOrgEntity
        //{
        //    TOrg t = await FirstOrDefaultAsync(i => i.Organisation.Id == orgId && i.Id == id, throwIfNotFound);
        //    return t;
        //}

        public T Find(Guid id, bool throwIfNotFound = false)
        {
            var result = DbSet.Find(id);

            if (throwIfNotFound && result == null)
                throw new ApplicationException(string.Format("No {0} could be found with provided id.", typeof(T).Name));

            return result;
        }

        public IEnumerable<T> Find(List<Guid> ids)
        {
            var result = DbSet.Where(o => ids.Contains(o.Id)).ToList();
            return result;
        }

        public async Task<T> FindAsync(Guid id, bool throwIfNotFound = false)
        {
            var result = await DbSet.FindAsync(id);

            if (throwIfNotFound && result == null)
                throw new ApplicationException(string.Format("No {0} could be found with provided id.", typeof(T).Name));

            return result;
        }

        public async Task<IEnumerable<T>> FindAsync(List<Guid> ids, bool throwIfNotFound = false)
        {
            var result = await DbSet.Where(o => ids.Contains(o.Id)).ToListAsync();

            int diff = ids.Count - result.Count;
            if (throwIfNotFound && diff != 0)
                throw new ApplicationException(string.Format("No {0} could be found for {1} ids.", typeof(T).Name, diff));

            return result;
        }

        public List<T> Find(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false)
        {
            var query = DbSet.Where(predicate);
            var result = query.ToList();

            if (throwIfNotFound && (result == null || result.Count == 0))
            {
                throw new ApplicationException(string.Format("No {0} could be found with provided criteria.", typeof(T).Name));
            }
            return result;
        }

        public List<T> Find<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, bool desc = false)
            where Tkey : struct
        {
            var query = DbSet.Where(predicate);
            query = desc ? query.OrderByDescending(orderByPredicate)
                : query.OrderBy(orderByPredicate);
            var result = query.ToList();
            return result;
        }

        public List<T> Find<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, int pageNo = 1, int pageSize = 10, bool desc = false)
            where Tkey : struct
        {
            var query = DbSet.Where(predicate);
            query = desc ? query.OrderByDescending(orderByPredicate)
                : query.OrderBy(orderByPredicate);
            query = query.Skip((pageNo - 1) * pageSize);
            query = query.Take(pageSize);
            var result = query.ToList();
            return result;
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false)
        {
            var query = DbSet.Where(predicate);
            var result = await query.ToListAsync();

            if (throwIfNotFound && (result == null || result.Count == 0))
            {
                throw new ApplicationException(string.Format("No {0} could be found with provided criteria.", typeof(T).Name));
            }
            return result;
        }

        public async Task<List<T>> FindAsync<Tkey>(Expression<Func<T, bool>> predicate
            , Expression<Func<T, Tkey>> orderByPredicate, bool desc = false)
            where Tkey : struct
        {
            var query = DbSet.Where(predicate);
            query = desc ? query.OrderByDescending(orderByPredicate)
                : query.OrderBy(orderByPredicate);
            var result = query.ToListAsync();
            return await result;
        }

        public async Task<List<T>> FindAsync<Tkey>(Expression<Func<T, bool>> predicate
            , Expression<Func<T, Tkey>> orderByPredicate
            , int pageNo = 1, int pageSize = 10, bool desc = false)
            where Tkey : struct
        {
            //Task<List<T>> query = _dbset.Where(predicate)
            //    .OrderBy(orderByPredicate)
            //    .Skip((pageNo - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToListAsync();
            //return await query;

            var query = DbSet.Where(predicate);
            query = desc ? query.OrderByDescending(orderByPredicate)
                : query.OrderBy(orderByPredicate);
            query = query.Skip((pageNo - 1) * pageSize);
            query = query.Take(pageSize);
            var result = query.ToListAsync();
            return await result;
        }

        public T FirstOrDefault(bool throwIfNotFound = false)
        {
            return FirstOrDefault(null, throwIfNotFound);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false)
        {
            T result = null;
            if (predicate == null)
            {
                result = DbSet.FirstOrDefault();
            }
            else
            {
                result = DbSet.FirstOrDefault(predicate);
            }

            if (throwIfNotFound && result == null)
            {
                throw new ApplicationException(string.Format("No {0} could be found with provided criteria.", typeof(T).Name));
            }
            return result;
        }

        public async Task<T> FirstOrDefaultAsync(bool throwIfNotFound = false)
        {
            return await FirstOrDefaultAsync(null, throwIfNotFound);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false)
        {
            T result = null;
            if (predicate == null)
            {
                result = await DbSet.FirstOrDefaultAsync();
            }
            else
            {
                result = await DbSet.FirstOrDefaultAsync(predicate);
            }

            if (throwIfNotFound && result == null)
            {
                throw new ApplicationException(string.Format("No {0} could be found with provided criteria.", typeof(T).Name));
            }
            return result;
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            bool exists = false;
            if (predicate == null)
            {
                exists = DbSet.Any();
            }
            else
            {
                exists = DbSet.Any(predicate);
            }
            return exists;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            bool exists = false;
            if (predicate == null)
            {
                exists = await DbSet.AnyAsync();
            }
            else
            {
                exists = await DbSet.AnyAsync(predicate);
            }
            return exists;
        }

        public virtual int Count()
        {
            return DbSet.Count();
        }

        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate);
        }

        public virtual async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }

        public virtual List<T> GetAll()
        {

            return DbSet.ToList<T>();
        }

        public List<T> GetAll<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct
        {
            return DbSet.OrderBy(orderByPredicate).ToList<T>();
        }

        public List<T> GetAllDesc<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct
        {
            return DbSet.OrderByDescending(orderByPredicate).ToList<T>();
        }

        public virtual async Task<List<T>> GetAllAsync()
        {

            return await DbSet.ToListAsync<T>();
        }

        public async Task<List<T>> GetAllAsync<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct
        {
            return await DbSet.OrderBy(orderByPredicate).ToListAsync<T>();
        }

        public async Task<List<T>> GetAllDescAsync<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct
        {
            return await DbSet.OrderByDescending(orderByPredicate).ToListAsync<T>();
        }

        //protected static IQueryable<T> AddFilter(Expression<Func<T, bool>> predicate, ItemSearchQuery searchQuery, IQueryable<T> query)
        //{
        //    //Func<T, bool> expressionDelegate = predicate.Compile();

        //    var q = query.Where(predicate);
        //    return q;
        //}

        protected static IQueryable<T> AddFilterForPaging(ISearchQuery<T> searchQuery, IQueryable<T> query)
        {
            if (searchQuery.PageSize > 0)
            {
                query = query.Skip((searchQuery.PageNo - 1) * searchQuery.PageSize);
                query = query.Take(searchQuery.PageSize);
            }
            return query;
        }

        protected static IQueryable<T> AddFilterOnEntityStatus(ISearchQuery<T> searchQuery, IQueryable<T> query)
        {
            //if (searchQuery.IncludeArchived)
            //    query = query.Where(o => o.EntityStatus == EntityStatus.Active || o.EntityStatus == EntityStatus.Archived);
            //else
            //    query = query.Where(o => o.EntityStatus == EntityStatus.Active);

            if (searchQuery.Status != null)
            {
                //query = query.Where(o => searchQuery.EntityStatus.Value == EntityStatus.Archived
                //    ? (o.EntityStatus == EntityStatus.Active || o.EntityStatus == EntityStatus.Archived)
                //    : (o.EntityStatus == EntityStatus.Active));

                query = query.Where(o => searchQuery.Status.Value == EntityStatus.Deleted
                        ? (o.EntityStatus == EntityStatus.Active || o.EntityStatus == EntityStatus.Deleted)
                        : (o.EntityStatus == EntityStatus.Active));
            }
            return query;
        }

        public virtual T Add(T entity)
        {
            return DbSet.Add(entity);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            Add(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            return DbSet.AddRange(entities);
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            AddRange(entities);
            await Context.SaveChangesAsync();
            return entities;
        }

        public virtual T Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        //public virtual async Task<T> UpdateAsync(T entity)
        //{
        //    Update(entity);
        //    await Context.SaveChangesAsync();
        //    return entity;
        //}

        //public T Archive(T entity)
        //{
        //    entity.EntityStatus = EntityStatus.Archived;
        //    return Update(entity);
        //}

        //public virtual IEnumerable<T> Archive(IEnumerable<T> entities)
        //{
        //    foreach (T entity in entities)
        //    {
        //        Archive(entity);
        //    }
        //    return entities;
        //}

        public T Activate(T entity)
        {
            entity.EntityStatus = EntityStatus.Active;
            return Update(entity);
        }

        public virtual IEnumerable<T> Activate(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                Activate(entity);
            }
            return entities;
        }

        public T Deactivate(T entity)
        {
            entity.EntityStatus = EntityStatus.Deactivated;
            return Update(entity);
        }

        public virtual IEnumerable<T> Deactivate(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                Deactivate(entity);
            }
            return entities;
        }

        public T SoftDelete(T entity)
        {
            entity.Delete();
            return Update(entity);
        }

        public virtual IEnumerable<T> SoftDelete(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                SoftDelete(entity);
            }
            return entities;
        }

        public virtual T Delete(T entity)
        {
            if (entity.IsLocked)
                throw new ApplicationException("This record is locked. Locked records cannot be deleted.");

            if (entity.HasChildRecords())
            {
                string name = entity.GetType().Name.ToLower();
                name = "record";
                throw new ApplicationException("The {0} cannot be deleted. It has some dependent data on the system. Please deactivate the {1} if it is not required any more.".FormatWith(name, name));
            }

            return DbSet.Remove(entity);
        }

        //public virtual async Task<T> DeleteAsync(T entity)
        //{
        //    T e = Delete(entity);
        //    await Context.SaveChangesAsync();
        //    return e;
        //}

        public virtual IEnumerable<T> Delete(IEnumerable<T> entities)
        {
            //return _dbset.RemoveRange(entities);
            foreach (T entity in entities)
            {
                Delete(entity);
            }
            return entities;
        }

        public virtual async Task<IEnumerable<T>> DeleteAsync(IEnumerable<T> entities)
        {
            var e = Delete(entities);
            await Context.SaveChangesAsync();
            return entities;
        }

        public T Delete(Guid id)
        {
            T entity = Find(id, true);
            Delete(entity);
            return entity;
        }

        public async Task<T> DeleteAsync(Guid id)
        {
            T entity = await FindAsync(id, true);
            Delete(entity);
            return entity;
        }

        public IEnumerable<T> Delete(List<Guid> ids)
        {
            var list = Find(ids);
            Delete(list);
            return list;
        }

        public async Task<IEnumerable<T>> DeleteAsync(List<Guid> ids)
        {
            var list = await FindAsync(ids);
            Delete(list);
            return list;
        }

        //public virtual void Save()
        //{
        //    Context.SaveChanges();
        //}

        public T Attach(T entity)
        {
            T t = DbSet.Attach(entity);
            return t;
        }


        public async Task<T> FirstOrDefaultAsync<Tkey>(Expression<Func<T, bool>> predicate
            , Expression<Func<T, Tkey>> orderByPredicate, bool desc = false) where Tkey : struct
        {
            var query = DbSet.Where(predicate);
            query = desc ? query.OrderByDescending(orderByPredicate)
                : query.OrderBy(orderByPredicate);
            var result = query.FirstOrDefaultAsync();
            return await result;
        }
    }
}
