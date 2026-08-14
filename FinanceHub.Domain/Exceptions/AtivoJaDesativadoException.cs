namespace FinanceHub.Domain.Exceptions
{
    public class AtivoJaDesativadoException : Exception
    {
        public AtivoJaDesativadoException(Guid id) : base($"O ativo '{id}' já está desativado.")
        {
        }
    }
}
