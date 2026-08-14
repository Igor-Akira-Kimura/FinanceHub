using FinanceHub.Domain.Enums;

namespace FinanceHub.Application.Requests
{
    public class CriarAtivoRequest
    {
        public string Nome { get; set; } = string.Empty;

        public string Ticker { get; set; } = string.Empty;

        public TipoAtivo Tipo { get; set; }

        public Guid BolsaId { get; set; }
    }
}
