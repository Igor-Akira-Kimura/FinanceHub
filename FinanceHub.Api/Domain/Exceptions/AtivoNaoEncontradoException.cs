namespace FinanceHub.Api.Domain.Exceptions
{
    public class AtivoNaoEncontradoException : Exception
    {
        public AtivoNaoEncontradoException(Guid id) : base($"O ativo com o ID '{id}' não foi encontrado.")
        {
        }
    }
}
