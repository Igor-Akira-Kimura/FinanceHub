namespace FinanceHub.Api.Domain.Exceptions
{
    public class CarteiraNaoEncontradaException : Exception
    {
        public CarteiraNaoEncontradaException(Guid id) : base($"A carteira com o id '{id}' não foi encontrada.")
        {
        }
    }
}
