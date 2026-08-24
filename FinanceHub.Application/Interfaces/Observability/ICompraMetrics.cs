namespace FinanceHub.Application.Interfaces.Observability;

public interface ICompraMetrics
{
    void CompraRealizada();

    void RegistrarDuracao(
        double duracaoMs);
}