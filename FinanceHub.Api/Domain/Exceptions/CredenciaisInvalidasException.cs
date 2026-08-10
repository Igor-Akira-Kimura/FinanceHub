namespace FinanceHub.Api.Exceptions;

public class CredenciaisInvalidasException : Exception
{
    public CredenciaisInvalidasException() : base("E-mail ou senha inválidos.")
    {
    }
}