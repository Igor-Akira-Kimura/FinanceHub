namespace FinanceHub.Domain.Exceptions
{
    public class CarteiraJaDesativadaException : Exception
    {
        public CarteiraJaDesativadaException(Guid carteiraId) : base($"A carteira com o ID '{carteiraId}' já está desativada.")
        {
        }
    }
}
