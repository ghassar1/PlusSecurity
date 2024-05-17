using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AcSys.ShiftManager.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //void MarkDirty(object entity);
        //void MarkNew(object entity);
        //void MarkDeleted(object entity);

        //void Commit();
        //Task CommitAsync();

        //void Rollback();

        bool HasChanges();

        void SaveChanges();
        void SaveChangesIfAny();

        Task SaveChangesAsync();
        Task SaveChangesIfAnyAsync();

        void RenewContext(bool saveChangesBeforeDisposal = false);

        void SetLogger(Action<string> action);

        void Reload<T>(T entity) where T : class;
        void Reload(object entity);
        Task ReloadAsync<T>(T entity) where T : class;
        Task ReloadAsync(object entity);

        void Reload<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class;

        Task ReloadAsync<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class;

        void Reload<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, TElement>> navigationProperty)
            where TEntity : class
            where TElement : class;

        Task ReloadAsync<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, TElement>> navigationProperty)
            where TEntity : class
            where TElement : class;
    }
}
