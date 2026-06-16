using BLL.Mapper;
using BLL.Service.Abstraction;
using BLL.Service.Impelementation;
using BLL.Settings;
using DAL.Database;
using DAL.Entities;
using DAL.UnitOfWork;
using pharmacy_system.Extensions;
using pharmacy_system.Filters;
using pharmacy_system.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace pharmacy_system
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            // Controllers
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Infrastructure (DbContext + Repos)
            builder.Services.AddApplicationServices(builder.Configuration);

            // Services (BLL)
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
  .AddJwtBearer(options =>
  {
      options.RequireHttpsMetadata = false;
      options.SaveToken = true;

      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,

          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(key),

          RoleClaimType = ClaimTypes.Role,
          NameClaimType = ClaimTypes.Name
      };

      // 👇 مهم جدًا (يمنع redirect لـ /Account/Login)
      options.Events = new JwtBearerEvents
      {
          OnChallenge = context =>
          {
              context.HandleResponse();
              context.Response.StatusCode = 401;
              context.Response.ContentType = "application/json";
              return context.Response.WriteAsync("Unauthorized");
          }
      };
  });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var userManager =
                    services.GetRequiredService<UserManager<ApplicationUser>>();

                var roleManager =
                    services.GetRequiredService<RoleManager<IdentityRole>>();

                DbInitializer.SeedAdminAsync(userManager, roleManager).Wait();
            }


            // Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middleware pipeline
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors("AllowAngular");
            app.UseAuthentication(); // جاهز للمستقبل (JWT)
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}