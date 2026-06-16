using DAL.Entities;

namespace BLL.Service.Abstraction
{
    public interface IEmailService
    {
        Task SendOrderEmailAsync(Order order);
    }
}