namespace FinanceHub.Domain.Exceptions
{
    public class UsuarioInativoException : Exception
    {
        public UsuarioInativoException(Guid id) : base($"O usuário '{id}' está inativo.")
        {
        }
    }
}
