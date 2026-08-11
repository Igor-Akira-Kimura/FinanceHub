using FinanceHub.Api.ExceptionHandling;
using FinanceHub.Api.ExceptionHandling.Handlers;

namespace FinanceHub.Api.DependencyInjection;

public static class ExceptionHandlerExtensions
{
    public static IServiceCollection AddExceptionHandlers(
        this IServiceCollection services)
    {
        services.AddSingleton<IExceptionHandler, ValidationExceptionHandler>();

        services.AddSingleton<IExceptionHandler, UsuarioNaoEncontradoExceptionHandler>();
        //services.AddSingleton<IExceptionHandler, UsuarioInativoExceptionHandler>();
        services.AddSingleton<IExceptionHandler, UsuarioJaDesativadoExceptionHandler>();

        services.AddSingleton<IExceptionHandler, EmailJaCadastradoExceptionHandler>();

        services.AddSingleton<IExceptionHandler, AtivoNaoEncontradoExceptionHandler>();
        //services.AddSingleton<IExceptionHandler, AtivoJaDesativadoExceptionHandler>();

        services.AddSingleton<IExceptionHandler, BolsaNaoEncontradaExceptionHandler>();
        services.AddSingleton<IExceptionHandler, BolsaJaDesativadaExceptionHandler>();

        services.AddSingleton<IExceptionHandler, CarteiraNaoEncontradaExceptionHandler>();
        services.AddSingleton<IExceptionHandler, CarteiraJaCadastradaExceptionHandler>();
        //services.AddSingleton<IExceptionHandler, CarteiraJaDesativadaExceptionHandler>();

        services.AddSingleton<IExceptionHandler, PosicaoNaoEncontradaExceptionHandler>();

        services.AddSingleton<IExceptionHandler, QuantidadeInsuficienteExceptionHandler>();

        services.AddSingleton<IExceptionHandler, CredenciaisInvalidasExceptionHandler>();

        return services;
    }
}