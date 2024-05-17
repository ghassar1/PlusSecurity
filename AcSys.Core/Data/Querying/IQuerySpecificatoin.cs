using AcSys.Core.Data.Specifications;

namespace AcSys.Core.Data.Querying
{
    public interface IQuerySpecificatoin<T>
    {
        ISpecification<T> ToSpec();
    }
}
