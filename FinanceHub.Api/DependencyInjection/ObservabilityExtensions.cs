using FinanceHub.Application.Interfaces.Observability;
using FinanceHub.Infrastructure.Observability;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FinanceHub.Api.DependencyInjection;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services)
    {
        services.AddSingleton<ICompraMetrics, CompraMetrics>();

        services
            .AddOpenTelemetry()

            // =========================================
            // METRICS
            // =========================================

            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter("FinanceHub")
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            })

            // =========================================
            // TRACING
            // =========================================

            .WithTracing(tracing =>
            {
                tracing
                    .AddSource("FinanceHub")
                    .AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddOtlpExporter();
            });

        return services;
    }
}