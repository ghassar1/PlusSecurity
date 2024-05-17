using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.UnitOfWork;

namespace AcSys.ShiftManager.Data.EF.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected ApplicationDbContext _context = null;

        public UnitOfWork(ApplicationDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context", "Context argument must be provided in UnitOfWork.");

            this._context = context;
        }

        public void SetLogger(Action<string> action)
        {
            this._context.Database.Log = action;
        }

        public void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public void SaveChangesIfAny()
        {
            if (!this.HasChanges()) return;

            this.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await this._context.SaveChangesAsync();
        }

        public async Task SaveChangesIfAnyAsync()
        {
            if (!this.HasChanges()) return;

            await this.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (this._context != null)
            {
                this._context.Dispose();
            }
        }

        public void RenewContext(bool saveChangesBeforeDisposal = false)
        {
            if (saveChangesBeforeDisposal)
            {
                if (this._context.ChangeTracker.HasChanges())
                {
                    this._context.SaveChanges();
                }
            }
            this._context.Dispose();

            this._context = new ApplicationDbContext();
        }

        public bool HasChanges()
        {
            return this._context.ChangeTracker.HasChanges();
        }

        public void Reload<T>(T entity) where T : class
        {
            this._context.Entry<T>(entity).Reload();
        }

        public void Reload(object entity)
        {
            this._context.Entry(entity).Reload();
        }

        public async Task ReloadAsync<T>(T entity) where T : class
        {
            await this._context.Entry<T>(entity).ReloadAsync();
        }

        public async Task ReloadAsync(object entity)
        {
            await this._context.Entry(entity).ReloadAsync();
        }

        public void Reload<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            this._context.Entry(entity).Collection<TElement>(navigationProperty).Load();
        }

        public async Task ReloadAsync<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            await this._context.Entry(entity).Collection<TElement>(navigationProperty).LoadAsync();
        }

        public void Reload<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, TElement>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            this._context.Entry(entity).Reference<TElement>(navigationProperty).Load();
        }

        public async Task ReloadAsync<TEntity, TElement>(TEntity entity,
            Expression<Func<TEntity, TElement>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            await this._context.Entry(entity).Reference<TElement>(navigationProperty).LoadAsync();
        }
    }
}
