namespace FinanceHub.Domain.Exceptions
{
    public class CarteiraJaCadastradaException : Exception
    {
        public CarteiraJaCadastradaException(string nome) : base($"Já existe uma carteira com o nome '{nome}'.")
        {
        }
    }
}
