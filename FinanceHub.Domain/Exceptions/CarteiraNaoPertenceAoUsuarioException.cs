namespace FinanceHub.Domain.Exceptions
{
    public class CarteiraNaoPertenceAoUsuarioException : Exception
    {
        public CarteiraNaoPertenceAoUsuarioException(Guid carteiraId)
            : base($"A carteira {carteiraId} não pertence ao usuário atual.")
        {
        }
    }
}