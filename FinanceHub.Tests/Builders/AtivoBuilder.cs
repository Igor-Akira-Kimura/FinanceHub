using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Domain.Enums;

namespace FinanceHub.Tests.Builders;

public class AtivoBuilder
{
    private string _nome = "PETROBRAS";

    private string _ticker = "PETR4";

    private TipoAtivo _tipo = TipoAtivo.Acao;

    private Guid _bolsaId = Guid.NewGuid();

    public AtivoBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public AtivoBuilder ComTicker(string ticker)
    {
        _ticker = ticker;
        return this;
    }

    public AtivoBuilder ComTipo(TipoAtivo tipo)
    {
        _tipo = tipo;
        return this;
    }

    public AtivoBuilder ComBolsa(Guid bolsaId)
    {
        _bolsaId = bolsaId;
        return this;
    }

    public Ativo Build()
    {
        return new Ativo(
            _nome,
            _ticker,
            _tipo,
            _bolsaId);
    }
}