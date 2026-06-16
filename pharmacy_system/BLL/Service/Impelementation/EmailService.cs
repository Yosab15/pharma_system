using BLL.Service.Abstraction;
using BLL.Settings;
using DAL.Entities;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BLL.Service.Impelementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendOrderEmailAsync(Order order)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.Email));
            email.To.Add(MailboxAddress.Parse(_settings.CompanyEmail));
            email.Subject = $"New Order #{order.Id}";

            var builder = new BodyBuilder();

            var itemsHtml = "";
            foreach (var item in order.OrderItems)
            {
                itemsHtml += $@"
                    <li>
                        Product Id: {item.ProductId}
                        - Qty: {item.Quantity}
                        - Price: {item.Price}
                    </li>";
            }

            builder.HtmlBody = $@"
                <h2>New Pharmacy Order</h2>
                <p><strong>Customer:</strong> {order.CustomerName}</p>
                <p><strong>Responsible:</strong> {order.ResponsibleName}</p>
                <p><strong>Phone:</strong> {order.PhoneNumber}</p>
                <p><strong>Address:</strong> {order.Address}</p>
                <p><strong>City:</strong> {order.City}</p>
                <p><strong>Notes:</strong> {order.Notes}</p>
                {(order.SupplyOrderImageUrl != null ? $"<p><strong>Supply Order:</strong> {order.SupplyOrderImageUrl}</p>" : "")}
                {(order.Latitude != null ? $"<p><strong>Location:</strong> {order.Latitude}, {order.Longitude}</p>" : "")}
                <hr/>
                <h3>Products:</h3>
                <ul>{itemsHtml}</ul>
                <hr/>
                <h3>Total: {order.TotalPrice} EGP</h3>
            ";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Email, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}