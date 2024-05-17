using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AcSys.Core.Data.Querying;
using AcSys.Core.Data.Specifications;

namespace AcSys.Core.Data.Repository
{
    public interface IGenericRepository<T> : IRepository, IDisposable
    {
        IQueryable<T> GetAsQueryable();

        //Task<T> FirstOrDefault(IQuerySpecificatoin<T> query);
        //Task<ISearchResult<T>> Find(IQuerySpecificatoin<T> query);

        Task<T> FirstOrDefault(ISpecification<T> spec);
        Task<ISearchResult<T>> Find(ISpecification<T> spec);


        Task<bool> Any(ISpecification<T> spec);

        Task<T> FirstOrDefault(ISearchQuery<T> query);
        Task<ISearchResult<T>> Find(ISearchQuery<T> query);
        Task<bool> Any(ISearchQuery<T> spec);
        Task<int> Count(ISearchQuery<T> query);
        Task<int> Count(ISpecification<T> spec);

        int Count();
        int Count(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        T Find(Guid id, bool throwIfNotFound = false);
        IEnumerable<T> Find(List<Guid> ids);
        Task<T> FindAsync(Guid id, bool throwIfNotFound = false);
        Task<IEnumerable<T>> FindAsync(List<Guid> ids, bool throwIfNotFound = false);



        List<T> Find(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false);
        List<T> Find<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, bool desc = false) where Tkey : struct;
        List<T> Find<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, int pageNo, int pageSize, bool desc = false) where Tkey : struct;

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false);
        Task<List<T>> FindAsync<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, bool desc = false) where Tkey : struct;
        Task<List<T>> FindAsync<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, int pageNo, int pageSize, bool desc = false) where Tkey : struct;



        T FirstOrDefault(bool throwIfNotFound = false);
        T FirstOrDefault(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false);
        Task<T> FirstOrDefaultAsync(bool throwIfNotFound = false);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool throwIfNotFound = false);
        Task<T> FirstOrDefaultAsync<Tkey>(Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderByPredicate, bool desc = false) where Tkey : struct;



        bool Any(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        List<T> GetAll();
        Task<List<T>> GetAllAsync();

        List<T> GetAll<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct;
        Task<List<T>> GetAllAsync<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct;

        List<T> GetAllDesc<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct;
        Task<List<T>> GetAllDescAsync<Tkey>(Expression<Func<T, Tkey>> orderByPredicate) where Tkey : struct;

        T Attach(T entity);

        T Add(T entity);
        Task<T> AddAsync(T entity);

        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        T Update(T entity);
        //Task<T> UpdateAsync(T entity);

        T Delete(T entity);
        //Task<T> DeleteAsync(T entity);

        IEnumerable<T> Delete(IEnumerable<T> entities);
        Task<IEnumerable<T>> DeleteAsync(IEnumerable<T> entities);

        T Delete(Guid id);
        Task<T> DeleteAsync(Guid id);

        IEnumerable<T> Delete(List<Guid> ids);
        Task<IEnumerable<T>> DeleteAsync(List<Guid> ids);

        //T Archive(T entity);
        //IEnumerable<T> Archive(IEnumerable<T> entities);

        T Activate(T entity);
        IEnumerable<T> Activate(IEnumerable<T> entities);

        T Deactivate(T entity);
        IEnumerable<T> Deactivate(IEnumerable<T> entities);

        T SoftDelete(T entity);
        IEnumerable<T> SoftDelete(IEnumerable<T> entities);
    }
}
