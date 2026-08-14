using FinanceHub.Domain.Entities;

namespace FinanceHub.Tests.Builders;

public class PosicaoBuilder
{
    private Guid _carteiraId = Guid.NewGuid();

    private Guid _ativoId = Guid.NewGuid();

    private decimal _quantidade = 10;

    private decimal _precoMedio = 20;

    public PosicaoBuilder ComCarteira(Guid carteiraId)
    {
        _carteiraId = carteiraId;
        return this;
    }

    public PosicaoBuilder ComAtivo(Guid ativoId)
    {
        _ativoId = ativoId;
        return this;
    }

    public PosicaoBuilder ComQuantidade(decimal quantidade)
    {
        _quantidade = quantidade;
        return this;
    }

    public PosicaoBuilder ComPrecoMedio(decimal preco)
    {
        _precoMedio = preco;
        return this;
    }

    public Posicao Build()
    {
        return new Posicao(
            _carteiraId,
            _ativoId,
            _quantidade,
            _precoMedio);
    }
}