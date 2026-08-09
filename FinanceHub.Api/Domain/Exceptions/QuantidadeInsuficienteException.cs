namespace FinanceHub.Api.Domain.Exceptions
{
    public class QuantidadeInsuficienteException : Exception
    {
        public QuantidadeInsuficienteException(Guid ativoId, decimal quantidadeDisponivel, decimal quantidadeSolicitada) : base($"A quantidade disponível do ativo com ID '{ativoId}' é insuficiente. Quantidade disponível: {quantidadeDisponivel}, Quantidade solicitada: {quantidadeSolicitada}.")
        {
        }
    }
}
