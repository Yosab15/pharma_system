
using DAL.Entities;

namespace DAL.Specifications.Orders
{
    public class OrderSpecification
        : BaseSpecification<Order>
    {
        public OrderSpecification(OrderSpecParams query)
        {
            // FILTER + SEARCH
            Criteria = o =>

                (string.IsNullOrEmpty(query.Search)
                || o.CustomerName.Contains(query.Search)
                || o.PhoneNumber.Contains(query.Search))

                &&

                (!query.Status.HasValue
                || o.Status == query.Status.Value)
                 &&
                (!query.City.HasValue  // ← جديد
                || o.City == query.City.Value);
                
            ;


            // INCLUDE
            AddInclude(o => o.OrderItems);

            // ORDERING
            AddOrderByDescending(o => o.OrderDate);

            // PAGINATION
            ApplyPaging(
                (query.PageNumber - 1) * query.PageSize,
                query.PageSize
            );
        }
    }
}