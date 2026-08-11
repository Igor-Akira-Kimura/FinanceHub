using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Services;

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
        services.AddScoped<IPosicaoService, PosicaoService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}