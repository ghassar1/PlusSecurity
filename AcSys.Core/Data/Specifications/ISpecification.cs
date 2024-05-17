using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AcSys.Core.Data.Querying;

namespace AcSys.Core.Data.Specifications
{
    //  http://blog.willbeattie.net/2011/02/specification-pattern-entity-framework.html
    //  http://stackoverflow.com/questions/25244030/specification-pattern-with-entity-framework-and-using-orderby-and-skip-take
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Predicate { get; }
        Func<IQueryable<T>, IOrderedQueryable<T>> SortPredicate { get; }
        Func<IQueryable<T>, IQueryable<T>> PostProcessPredicate { get; }

        IFetchStrategy<T> FetchStrategy { get; }

        string IncludePath { get; set; }

        bool IsSatisfiedBy(T entity);
        T SatisfyingItemFrom(IQueryable<T> query);
        //IQueryable<T> SatisfyingItemsFrom(IQueryable<T> query);
        Task<ISearchResult<T>> SatisfyingItemsFrom(IQueryable<T> query);
        Task<int> Count(IQueryable<T> query);

        ISpecification<T> And(ISpecification<T> specification);

        ISpecification<T> And(Expression<Func<T, bool>> predicate);

        ISpecification<T> Or(ISpecification<T> specification);

        ISpecification<T> Or(Expression<Func<T, bool>> predicate);

        ISpecification<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> property);
        ISpecification<T> OrderByDescending<TProperty>(Expression<Func<T, TProperty>> property);
        ISpecification<T> Take(int amount);
        ISpecification<T> Skip(int amount);
    }
}
