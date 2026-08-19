namespace FinanceHub.Application.Requests
{
    public class VenderAtivoRequest
    {
        public Guid CarteiraId { get; set; }

        public Guid AtivoId { get; set; }

        public decimal Quantidade { get; set; }
    }
}
