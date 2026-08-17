namespace FinanceHub.Domain.Exceptions;

public class SaldoInsuficienteException : Exception
{
    public Guid CarteiraId { get; }

    public decimal ValorSolicitado { get; }

    public SaldoInsuficienteException(
        Guid carteiraId,
        decimal valorSolicitado)
        : base("Saldo insuficiente para realizar a operação.")
    {
        CarteiraId = carteiraId;
        ValorSolicitado = valorSolicitado;
    }
}