using System;
using System.Linq.Expressions;

namespace JumpsellerSync.BusinessLogic.Filter
{
    public class DomainFilter<TModel>
        where TModel : class
    {
        private readonly string filter;

        public delegate DomainFilter<TModel> Factory(string filter);

        public DomainFilter(string filter)
        { this.filter = filter; }

        public Expression<Func<TModel, bool>> GetConditions()
        {
            return model => true;
        }
    }
}
