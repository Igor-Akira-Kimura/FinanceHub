namespace FinanceHub.Domain.Exceptions
{
    public class UsuarioJaDesativadoException : Exception
    {
        public UsuarioJaDesativadoException(Guid id) : base($"O usuário com ID '{id}' já está desativado.")
        {
        }
    }
}
