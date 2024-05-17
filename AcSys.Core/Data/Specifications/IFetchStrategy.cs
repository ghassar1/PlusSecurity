using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AcSys.Core.Data.Specifications
{
    public interface IFetchStrategy<T>
    {
        IEnumerable<string> IncludePaths { get; }

        IFetchStrategy<T> Include(Expression<Func<T, object>> path);

        IFetchStrategy<T> Include(string path);
    }
}
