using System.Diagnostics.Metrics;
using FinanceHub.Application.Interfaces.Observability;

namespace FinanceHub.Infrastructure.Observability;

public class CompraMetrics : ICompraMetrics
{
    private readonly Counter<long> _comprasRealizadas;

    private readonly Histogram<double> _duracaoCompra;

    public CompraMetrics(
        IMeterFactory meterFactory)
    {
        var meter =
            meterFactory.Create("FinanceHub");

        _comprasRealizadas =
            meter.CreateCounter<long>(
                "financehub.compras.realizadas");

        _duracaoCompra =
            meter.CreateHistogram<double>(
                "financehub.compras.duracao",
                unit: "ms",
                description:
                    "Duração das compras em milissegundos.");
    }

    public void CompraRealizada()
    {
        _comprasRealizadas.Add(1);
    }

    public void RegistrarDuracao(
        double duracaoMs)
    {
        _duracaoCompra.Record(duracaoMs);
    }
}