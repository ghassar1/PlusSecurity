using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace AcSys.Core.Data.Specifications
{
    // http://stackoverflow.com/a/5431309/3423802
    internal class ParameterVisitor : ExpressionVisitor
    {
        readonly ReadOnlyCollection<ParameterExpression> from, to;
        public ParameterVisitor(
            ReadOnlyCollection<ParameterExpression> from,
            ReadOnlyCollection<ParameterExpression> to)
        {
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (from.Count != to.Count) throw new InvalidOperationException(
                  "Parameter lengths must match");
            this.from = from;
            this.to = to;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            for (int i = 0; i < from.Count; i++)
            {
                if (node == from[i]) return to[i];
            }
            return node;
        }
    }
}
