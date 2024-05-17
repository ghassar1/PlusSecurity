using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AcSys.Core.Data.Specifications
{
    public class FetchStrategyBase<T> : IFetchStrategy<T>
    {
        private readonly IList<string> properties;

        public FetchStrategyBase()
        {
            properties = new List<string>();
        }

        #region IFetchStrategy<T> Members

        public IEnumerable<string> IncludePaths
        {
            get { return properties; }
        }

        public IFetchStrategy<T> Include(Expression<Func<T, object>> path)
        {
            properties.Add(path.ToPropertyName());
            return this;
        }

        public IFetchStrategy<T> Include(string path)
        {
            properties.Add(path);
            return this;
        }

        #endregion
    }

    public static class Extensions
    {
        public static string ToPropertyName<T>(this Expression<Func<T, object>> selector)
        {
            var me = selector.Body as MemberExpression;
            if (me == null)
            {
                throw new ArgumentException("MemberException expected.");
            }

            var propertyName = me.ToString().Remove(0, 2);
            return propertyName;
        }
    }
}
