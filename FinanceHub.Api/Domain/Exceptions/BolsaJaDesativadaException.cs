namespace FinanceHub.Api.Domain.Exceptions
{
    public class BolsaJaDesativadaException : Exception
    {
        public BolsaJaDesativadaException(Guid id) : base($"A bolsa '{id}' já está desativada.")
        {
        }
    }
}
