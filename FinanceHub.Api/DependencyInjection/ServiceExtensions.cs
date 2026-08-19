using FinanceHub.Api.Authentication;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Services;
using FinanceHub.Infrastructure.Authentication;
using FinanceHub.Infrastructure.Security;

namespace FinanceHub.Api.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAtivoService, AtivoService>();
        services.AddScoped<IBolsaService, BolsaService>();
        services.AddScoped<ICarteiraService, CarteiraService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}