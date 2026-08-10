using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Tests.Builders;

public class CarteiraBuilder
{
    private string _nome = "Carteira Principal";
    private Guid _usuarioId = Guid.NewGuid();

    public CarteiraBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public CarteiraBuilder ComUsuario(Guid usuarioId)
    {
        _usuarioId = usuarioId;
        return this;
    }

    public Carteira Build()
    {
        return new Carteira(
            _nome,
            _usuarioId);
    }
}