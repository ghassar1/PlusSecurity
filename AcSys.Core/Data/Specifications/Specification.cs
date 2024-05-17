using System;
using System.Linq.Expressions;

namespace AcSys.Core.Data.Specifications
{
    public class Specification<T> : SpecificationBase<T>
    {
        public Specification()
            : base()
        {
        }

        //public Specification(Expression perdicate)
        //    : base(perdicate)
        //{
        //}

        //public Specification(BinaryExpression binaryPredicate)
        //{
        //    BinaryPredicate = binaryPredicate;
        //}

        public Specification(Expression<Func<T, bool>> perdicate)
            : base(perdicate)
        {
        }
    }
}
