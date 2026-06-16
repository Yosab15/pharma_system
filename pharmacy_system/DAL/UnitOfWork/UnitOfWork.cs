using DAL.Database;
using DAL.Entities;
using DAL.Repo.Abstraction;
using DAL.Repo.Impelementation;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepo<Product> Products { get; }

        public IGenericRepo<Category> Categories { get; }

        public IGenericRepo<Order> Orders { get; }

        public IGenericRepo<OrderItem> OrderItems { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Products = new GenericRepo<Product>(_context);

            Categories = new GenericRepo<Category>(_context);

            Orders = new GenericRepo<Order>(_context);

            OrderItems = new GenericRepo<OrderItem>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}