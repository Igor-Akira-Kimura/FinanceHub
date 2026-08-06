
using FinanceHub.Api.Repositories;
using FinanceHub.Api.Repositories.Interfaces;
using FinanceHub.Api.Services;
using FinanceHub.Api.Services.Interfaces;
using FinanceHub.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // DI Services
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();

            // DI Repositories
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // DI DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
