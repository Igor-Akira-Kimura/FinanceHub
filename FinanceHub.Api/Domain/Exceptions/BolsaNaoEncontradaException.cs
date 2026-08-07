namespace FinanceHub.Api.Domain.Exceptions
{
    public class BolsaNaoEncontradaException : Exception
    {
        public BolsaNaoEncontradaException(Guid id) : base($"A bolsa com o ID '{id}' não foi encontrada.")
        {
        }
    }
}
