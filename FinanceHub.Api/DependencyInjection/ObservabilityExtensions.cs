using FinanceHub.Application.Interfaces.Observability;
using FinanceHub.Infrastructure.Observability;
using OpenTelemetry.Metrics;

namespace FinanceHub.Api.DependencyInjection;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services)
    {
        services.AddSingleton<ICompraMetrics, CompraMetrics>();

        services
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter("FinanceHub")
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });

        return services;
    }
}