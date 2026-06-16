using System.Linq.Expressions;

namespace DAL.Specifications
{
    public class BaseSpecification<T>
        : ISpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; set; }

        public List<Expression<Func<T, object>>> Includes { get; }
            = new();

        public Expression<Func<T, object>>? OrderBy { get; set; }

        public Expression<Func<T, object>>? OrderByDescending { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public bool IsPagingEnabled { get; set; }

        public BaseSpecification()
        {

        }

        public BaseSpecification(
            Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(
            Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddOrderBy(
            Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(
            Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;

            Take = take;

            IsPagingEnabled = true;
        }
    }
}