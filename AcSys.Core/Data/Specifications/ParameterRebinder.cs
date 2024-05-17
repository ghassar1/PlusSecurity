using System.Collections.Generic;
using System.Linq.Expressions;

namespace AcSys.Core.Data.Specifications
{
    // https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/
    // https://chriscavanagh.wordpress.com/2007/11/04/linq-expression-visitor/
    public class ParameterRebinder : ExpressionVisitor
    {

        #region Private Fields

        readonly Dictionary<ParameterExpression, ParameterExpression> map;

        #endregion Private Fields

        #region Public Constructors

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        #endregion Public Constructors

        #region Public Methods

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }

        #endregion Protected Methods
    }
}
