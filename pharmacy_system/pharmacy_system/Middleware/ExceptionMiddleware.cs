using System.Net;
using System.Text.Json;

namespace pharmacy_system.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var message = GetMessage(ex);

                var response = new
                {
                    Success = false,
                    Message = message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }

        private string GetMessage(Exception ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;

            if (message.Contains("FOREIGN KEY"))
                return "Invalid Category Id";
            if (message.Contains("UNIQUE"))
                return "Duplicate value not allowed";
            if (message.Contains("Cannot insert"))
                return "Invalid data provided";
            if (message.Contains("Cannot delete category with existing products"))
                return "Cannot delete category with existing products";

            return message;
        }
    }
}