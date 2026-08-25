using System.Diagnostics.Metrics;

namespace FinanceHub.Application.Observability;

public static class FinanceHubMetrics
{
    public static readonly Meter Meter =
        new("FinanceHub.Application");

    public static readonly Histogram<double>
        CompraDuration =
            Meter.CreateHistogram<double>(
                "financehub.compra.duration",
                unit: "ms",
                description: "Tempo necessário para processar uma compra de ativo.");
}