using System.Collections.Generic;

namespace AcSys.Core.Data.Querying
{
    public interface IListResult<T>
    {
        List<T> Records { get; set; }
    }
}
