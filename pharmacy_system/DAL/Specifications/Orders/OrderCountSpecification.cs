using DAL.Entities;

namespace DAL.Specifications.Orders
{
    public class OrderCountSpecification
        : BaseSpecification<Order>
    {
        public OrderCountSpecification(OrderSpecParams query)
        {
            Criteria = o =>
                (string.IsNullOrEmpty(query.Search)
                || o.CustomerName.Contains(query.Search)
                || o.PhoneNumber.Contains(query.Search))
                &&
                (!query.Status.HasValue
                || o.Status == query.Status.Value)
                &&
                (!query.City.HasValue
                || o.City == query.City.Value);
            // بدون ApplyPaging عشان يعد الكل
        }
    }
}