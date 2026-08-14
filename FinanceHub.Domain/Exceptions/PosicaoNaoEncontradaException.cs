namespace FinanceHub.Domain.Exceptions
{
    public class PosicaoNaoEncontradaException : Exception
    {
        public PosicaoNaoEncontradaException(Guid ativoId) : base($"Não existe posição para o ativo '{ativoId}'.")
        {
        }
    }
}
