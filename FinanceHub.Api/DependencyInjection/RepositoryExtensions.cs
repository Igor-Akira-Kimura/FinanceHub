using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Infrastructure.Repositories;

namespace FinanceHub.Api.DependencyInjection;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAtivoRepository, AtivoRepository>();
        services.AddScoped<IBolsaRepository, BolsaRepository>();
        services.AddScoped<ICarteiraRepository, CarteiraRepository>();
        services.AddScoped<IPosicaoRepository, PosicaoRepository>();
        services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}