using FinanceHub.Domain.Entities;

namespace FinanceHub.Tests.Builders;

public class UsuarioBuilder
{
    private string _nome = "Igor";
    private string _email = "igor@email.com";
    private string _senhaHash = "HASH";

    public UsuarioBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public UsuarioBuilder ComEmail(string email)
    {
        _email = email;
        return this;
    }

    public UsuarioBuilder ComSenhaHash(string senhaHash)
    {
        _senhaHash = senhaHash;
        return this;
    }

    public Usuario Build()
    {
        return new Usuario(
            _nome,
            _email,
            _senhaHash);
    }
}