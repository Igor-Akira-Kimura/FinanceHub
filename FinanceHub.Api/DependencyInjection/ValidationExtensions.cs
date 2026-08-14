using FinanceHub.Application.Validators;
using FluentValidation;

namespace FinanceHub.Api.DependencyInjection;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidation(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CriarUsuarioRequestValidator>();

        return services;
    }
}