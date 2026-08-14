namespace FinanceHub.Domain.Exceptions
{
    public class EmailJaCadastradoException : Exception
    {
        public EmailJaCadastradoException(string email) : base($"Já existe um usuário cadastrado com o e-mail '{email}'.")
        {
        }
    }
}
