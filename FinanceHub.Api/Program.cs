using FinanceHub.Api.DependencyInjection;
using FinanceHub.Api.ExceptionHandling;

namespace FinanceHub.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddHttpContextAccessor();

        builder.Services
            .AddSwaggerDocumentation()
            .AddValidation()
            .AddApplicationServices()
            .AddRepositories()
            .AddDatabase(builder.Configuration)
            .AddJwtAuthentication(builder.Configuration)
            .AddExceptionHandlers();

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}