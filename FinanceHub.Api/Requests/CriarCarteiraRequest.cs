namespace FinanceHub.Api.Requests
{
    public class CriarCarteiraRequest
    {
        public string Nome { get; set; } = string.Empty;

        public Guid UsuarioId { get; set; }
    }
}
