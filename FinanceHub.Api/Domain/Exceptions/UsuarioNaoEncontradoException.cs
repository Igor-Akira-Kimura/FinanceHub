namespace FinanceHub.Api.Domain.Exceptions
{
    public class UsuarioNaoEncontradoException : Exception
    {
        public UsuarioNaoEncontradoException(Guid id) : base($"Usuário com o ID '{id}' não foi encontrado.")
        {
        }

        public UsuarioNaoEncontradoException(string email) : base($"Usuário com email '{email}' não encontrado.")
        {
        }
    }
}
