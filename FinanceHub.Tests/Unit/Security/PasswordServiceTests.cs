using FinanceHub.Infrastructure.Security;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Security;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService;

    public PasswordServiceTests()
    {
        _passwordService = new PasswordService();
    }

    [Fact]
    public void Hash_DeveGerarHashDiferenteDaSenha()
    {
        const string senha = "Senha123!";

        var hash =
            _passwordService.Hash(senha);

        hash.Should()
            .NotBeNullOrWhiteSpace();

        hash.Should()
            .NotBe(senha);
    }

    [Fact]
    public void Hash_DeveGerarHashesDiferentesParaMesmaSenha()
    {
        const string senha = "Senha123!";

        var hash1 =
            _passwordService.Hash(senha);

        var hash2 =
            _passwordService.Hash(senha);

        hash1.Should()
            .NotBe(hash2);
    }

    [Fact]
    public void Verify_ComSenhaCorreta_DeveRetornarTrue()
    {
        const string senha = "Senha123!";

        var hash =
            _passwordService.Hash(senha);

        var resultado =
            _passwordService.Verify(
                senha,
                hash);

        resultado.Should()
            .BeTrue();
    }

    [Fact]
    public void Verify_ComSenhaIncorreta_DeveRetornarFalse()
    {
        const string senha = "Senha123!";
        const string senhaErrada = "SenhaErrada!";

        var hash =
            _passwordService.Hash(senha);

        var resultado =
            _passwordService.Verify(
                senhaErrada,
                hash);

        resultado.Should()
            .BeFalse();
    }
}