using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AcSys.Core.Data.Querying;
using AcSys.Core.Extensions;

namespace AcSys.Core.Data.Specifications
{
    //  http://blog.willbeattie.net/2011/02/specification-pattern-entity-framework.html
    //  http://stackoverflow.com/questions/25244030/specification-pattern-with-entity-framework-and-using-orderby-and-skip-take
    //  https://json.codes/blog/generic-repository-and-the-specification-pattern/
    //  http://stackoverflow.com/a/6736589/3423802
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        protected SpecificationBase()
        {
            FetchStrategy = new FetchStrategyBase<T>();
        }

        //public SpecificationBase(Expression predicate)
        //    : this()
        //{
        //    ExpressionPredicate = predicate;
        //    if (predicate is BinaryExpression)
        //    {
        //        Predicate = null;
        //        BinaryPredicate = predicate as BinaryExpression;
        //    }
        //    else if (predicate is Expression<Func<T, bool>>)
        //    {
        //        Predicate = predicate as Expression<Func<T, bool>>;
        //        BinaryPredicate = null;
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Invalid predicate type.");
        //    }
        //}

        //public SpecificationBase(BinaryExpression predicate)
        //    : this()
        //{
        //    ExpressionPredicate = predicate;
        //    Predicate = null;
        //    BinaryPredicate = predicate;
        //}

        public SpecificationBase(Expression<Func<T, bool>> predicate)
            : this()
        {
            //ExpressionPredicate = predicate;
            Predicate = predicate;
            //BinaryPredicate = null;
        }

        //public Expression ExpressionPredicate { get; set; }

        //public BinaryExpression BinaryPredicate { get; set; }

        public Expression<Func<T, bool>> Predicate { get; protected set; }

        public Func<IQueryable<T>, IOrderedQueryable<T>> SortPredicate { get; protected set; }

        public Func<IQueryable<T>, IQueryable<T>> PostProcessPredicate { get; protected set; }

        public IFetchStrategy<T> FetchStrategy { get; protected set; }

        public string IncludePath { get; set; }

        public bool IsSatisfiedBy(T entity)
        {
            return new[] { entity }.AsQueryable().Any(Predicate);
        }

        public T SatisfyingItemFrom(IQueryable<T> query)
        {
            //return query.Where(Predicate).SingleOrDefault();
            //return Prepare(query).SingleOrDefault();

            IQueryable<T> filtered = query;

            if (Predicate != null)
                filtered = query.Where(Predicate);

            IOrderedQueryable<T> sorted = null;
            if (SortPredicate != null)
                sorted = SortPredicate(filtered);

            if (PostProcessPredicate != null)
                filtered = PostProcessPredicate(sorted ?? filtered);

            var results = filtered.FirstOrDefault();
            return results;
        }

        //public IQueryable<T> SatisfyingItemsFrom(IQueryable<T> query)
        public async Task<ISearchResult<T>> SatisfyingItemsFrom(IQueryable<T> query)
        {
            //return query.Where(Predicate);
            //return Prepare(query);

            ISearchResult<T> results = new SearchResults<T>();
            
            IQueryable<T> filtered = query;
            
            if (Predicate != null)
                filtered = filtered.Where(Predicate);

            results.TotalRecords = await filtered.CountAsync();

            IOrderedQueryable<T> sorted = null;
            if (SortPredicate != null)
                sorted = SortPredicate(filtered);

            if (PostProcessPredicate != null)
                filtered = PostProcessPredicate(sorted ?? filtered);

            results.FilteredRecords = await filtered.CountAsync();

            if (IncludePath.IsNotNullOrWhiteSpace())
                filtered = filtered.Include(IncludePath);

            results.Records = await filtered.ToListAsync();

            return results;
        }

        public async Task<int> Count(IQueryable<T> query)
        {
            IQueryable<T> filtered = query;

            if (Predicate != null)
                filtered = query.Where(Predicate);

            int results = await filtered.CountAsync();
            return results;
        }

        //IQueryable<T> Prepare(IQueryable<T> query)
        //{
        //    IQueryable<T> filtered = query;

        //    if (Predicate != null)
        //        filtered = query.Where(Predicate);

        //    IOrderedQueryable<T> sorted = null;
        //    if (SortPredicate != null)
        //        sorted = SortPredicate(filtered);

        //    if (PostProcessPredicate != null)
        //        filtered = PostProcessPredicate(sorted ?? filtered);

        //    return filtered;
        //}

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return And(specification.Predicate);
        }

        public ISpecification<T> And(Expression<Func<T, bool>> otherPredicate)
        {
            if (otherPredicate == null) return this;

            if (Predicate == null) return new Specification<T>(otherPredicate);

            //return new Specification<T>(Expression.And(Predicate, otherPredicate) as Expression<Func<T, bool>>);

            // Following code throws "ArgumentException: Incorrect number of parameters supplied for lambda declaration" if multiple arguments are used in the lambda predicates
            //// http://stackoverflow.com/a/6736589/3423802
            //var param = Expression.Parameter(typeof(T));
            //var body = Expression.AndAlso(Predicate.Body, otherPredicate.Body);
            //body = (BinaryExpression)new ParameterReplacer(param).Visit(body);
            //Predicate = Expression.Lambda<Func<T, bool>>(body, param);

            // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
            Predicate = Predicate.And(otherPredicate);

            return this;
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return Or(specification.Predicate);
        }

        public ISpecification<T> Or(Expression<Func<T, bool>> otherPredicate)
        {
            if (otherPredicate == null) return this;

            if (Predicate == null) return new Specification<T>(otherPredicate);

            //var ex = Expression.Or(Predicate.Body, otherPredicate.Body);   //as Expression<Func<T, bool>>;
            //return new Specification<T>(ex);

            // Following code throws "ArgumentException: Incorrect number of parameters supplied for lambda declaration" if multiple arguments are used in the lambda predicates
            //// http://stackoverflow.com/a/6736589/3423802
            //var param = Expression.Parameter(typeof(T));
            //var body = Expression.OrElse(Predicate.Body, otherPredicate.Body);
            //body = (BinaryExpression)new ParameterReplacer(param).Visit(body);
            //Predicate = Expression.Lambda<Func<T, bool>>(body, param);

            // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
            Predicate = Predicate.Or(otherPredicate);
            return this;
        }

        public ISpecification<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var newSpecification = new Specification<T>(Predicate) { PostProcessPredicate = PostProcessPredicate };
            if (SortPredicate != null)
            {
                newSpecification.SortPredicate = items => SortPredicate(items).ThenBy(property);
            }
            else
            {
                newSpecification.SortPredicate = items => items.OrderBy(property);
            }
            return newSpecification;
        }

        public ISpecification<T> OrderByDescending<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var newSpecification = new Specification<T>(Predicate) { PostProcessPredicate = PostProcessPredicate };
            if (SortPredicate != null)
            {
                newSpecification.SortPredicate = items => SortPredicate(items).ThenBy(property);
            }
            else
            {
                newSpecification.SortPredicate = items => items.OrderByDescending(property);
            }
            return newSpecification;
        }

        public ISpecification<T> Take(int amount)
        {
            var newSpecification = new Specification<T>(Predicate) { SortPredicate = SortPredicate };
            if (PostProcessPredicate != null)
            {
                newSpecification.PostProcessPredicate = items => PostProcessPredicate(items).Take(amount);
            }
            else
            {
                newSpecification.PostProcessPredicate = items => items.Take(amount);
            }
            return newSpecification;
        }

        public ISpecification<T> Skip(int amount)
        {
            var newSpecification = new Specification<T>(Predicate) { SortPredicate = SortPredicate };
            if (PostProcessPredicate != null)
            {
                newSpecification.PostProcessPredicate = items => PostProcessPredicate(items).Skip(amount);
            }
            else
            {
                newSpecification.PostProcessPredicate = items => items.Skip(amount);
            }
            return newSpecification;
        }
    }
}
