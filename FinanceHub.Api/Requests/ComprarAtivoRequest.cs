namespace FinanceHub.Api.Application.Requests.Carteiras;

public class ComprarAtivoRequest
{
    public Guid CarteiraId { get; set; }

    public Guid AtivoId { get; set; }

    public decimal Quantidade { get; set; }

    public decimal Preco { get; set; }
}