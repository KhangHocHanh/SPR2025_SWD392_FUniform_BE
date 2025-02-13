
using ClothingCustomization.Data;
using ClothingCustomization.Repository;
using Microsoft.EntityFrameworkCore;

namespace ClothingCustomization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Dependence Injection
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            // Inject IHttpContextAccessor in UserController
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<ClothesCusShopContext>(options =>
            {
                var connection = builder.Configuration.GetConnectionString("DefaultConnectionStrings");
                options.UseSqlServer(connection);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllers();

            app.Run();
        }
    }
}
