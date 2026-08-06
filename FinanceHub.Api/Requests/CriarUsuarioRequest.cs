namespace FinanceHub.Api.Requests
{
    public class CriarUsuarioRequest
    {
        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;
    }
}