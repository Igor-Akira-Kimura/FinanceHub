using FinanceHub.Application.Requests.Auth;
using FinanceHub.Application.Validators;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator;

    public LoginRequestValidatorTests()
    {
        _validator =
            new LoginRequestValidator();
    }

    [Fact]
    public void RequestValido_DeveSerValido()
    {
        var request =
            new LoginRequest
            {
                Email = "usuario@test.com",
                Senha = "Senha123!"
            };

        var result =
            _validator.Validate(request);

        result.IsValid
            .Should()
            .BeTrue();
    }

    [Fact]
    public void EmailVazio_DeveSerInvalido()
    {
        var request =
            new LoginRequest
            {
                Email = "",
                Senha = "Senha123!"
            };

        var result =
            _validator.Validate(request);

        result.IsValid
            .Should()
            .BeFalse();
    }

    [Fact]
    public void EmailInvalido_DeveSerInvalido()
    {
        var request =
            new LoginRequest
            {
                Email = "email-invalido",
                Senha = "Senha123!"
            };

        var result =
            _validator.Validate(request);

        result.IsValid
            .Should()
            .BeFalse();
    }

    [Fact]
    public void SenhaVazia_DeveSerInvalida()
    {
        var request =
            new LoginRequest
            {
                Email = "usuario@test.com",
                Senha = ""
            };

        var result =
            _validator.Validate(request);

        result.IsValid
            .Should()
            .BeFalse();
    }
}