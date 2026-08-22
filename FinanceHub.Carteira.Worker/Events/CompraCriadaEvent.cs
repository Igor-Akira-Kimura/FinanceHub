public class CompraCriadaEvent
{
    public Guid EventId { get; }

    public Guid CompraId { get; }

    public Guid CarteiraId { get; }

    public Guid AtivoId { get; }

    public decimal Quantidade { get; }

    public decimal Preco { get; }

    public CompraCriadaEvent(
        Guid compraId,
        Guid carteiraId,
        Guid ativoId,
        decimal quantidade,
        decimal preco)
    {
        EventId = Guid.NewGuid();

        CompraId = compraId;
        CarteiraId = carteiraId;
        AtivoId = ativoId;
        Quantidade = quantidade;
        Preco = preco;
    }
}