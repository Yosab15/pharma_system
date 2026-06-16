using DAL.Database;
using DAL.Repo.Abstraction;
using DAL.Repo.Impelementation;
using Microsoft.EntityFrameworkCore;

namespace pharmacy_system.Extensions
{   
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                ));

            // Generic Repo
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));


            // Repositories
            services.AddScoped<IProductRepo, ProductRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<IOrderRepo, OrderRepo>();

            return services;
        }
    }
}
