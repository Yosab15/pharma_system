using DAL.Entities;
using DAL.Repo.Abstraction;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepo<Product> Products { get; }

        IGenericRepo<Category> Categories { get; }

        IGenericRepo<Order> Orders { get; }

        IGenericRepo<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync();
    }
}