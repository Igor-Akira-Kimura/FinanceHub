namespace FinanceHub.Domain.Exceptions
{
    public class TickerJaCadastradoException : Exception
    {
        public TickerJaCadastradoException(string ticker) : base($"O ticker '{ticker}' já está cadastrado.")
        {
        }
    }
}
