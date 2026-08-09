
using FinanceHub.Api.Application.Requests.Carteiras;
using FinanceHub.Api.Data;
using FinanceHub.Api.Interfaces.Repositories;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Middlewares;
using FinanceHub.Api.Repositories;
using FinanceHub.Api.Services;
using FinanceHub.Api.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;

namespace FinanceHub.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddValidatorsFromAssemblyContaining<CriarUsuarioRequestValidator>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // DI Services
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IAtivoService, AtivoService>();
            builder.Services.AddScoped<IBolsaService, BolsaService>();
            builder.Services.AddScoped<ICarteiraService, CarteiraService>();
            builder.Services.AddScoped<IPosicaoService, PosicaoService>();

            // DI Repositories
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAtivoRepository, AtivoRepository>();
            builder.Services.AddScoped<IBolsaRepository, BolsaRepository>();
            builder.Services.AddScoped<ICarteiraRepository, CarteiraRepository>();
            builder.Services.AddScoped<IPosicaoRepository, PosicaoRepository>();
            builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

            // DI DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

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
