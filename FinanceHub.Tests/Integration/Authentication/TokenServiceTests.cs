using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Authentication;
using FinanceHub.Infrastructure.Configurations;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceHub.Tests.Integration.Authentication;

public class TokenServiceTests
{
    private const string Key =
        "SuaChaveSuperSecretaComNoMinimo32Caracteres123!";

    private const string Issuer =
        "FinanceHub.Api";

    private const string Audience =
        "FinanceHub.Client";

    [Fact]
    public void
        GerarToken_DeveRetornarToken()
    {
        var service =
            CriarService();

        var usuario =
            CriarUsuario();

        var token =
            service.GerarToken(usuario);

        token
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void
        GerarToken_DeveConterUserId()
    {
        var service =
            CriarService();

        var usuario =
            CriarUsuario();

        var token =
            service.GerarToken(usuario);

        var jwt =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

        var claim =
            jwt.Claims.First(
                x =>
                    x.Type ==
                    ClaimTypes.NameIdentifier);

        claim.Value
            .Should()
            .Be(usuario.Id.ToString());
    }

    [Fact]
    public void
        GerarToken_DeveConterEmail()
    {
        var service =
            CriarService();

        var usuario =
            CriarUsuario();

        var token =
            service.GerarToken(usuario);

        var jwt =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

        var claim =
            jwt.Claims.First(
                x =>
                    x.Type ==
                    ClaimTypes.Email);

        claim.Value
            .Should()
            .Be(usuario.Email);
    }

    [Fact]
    public void
        GerarToken_DeveConterNome()
    {
        var service =
            CriarService();

        var usuario =
            CriarUsuario();

        var token =
            service.GerarToken(usuario);

        var jwt =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

        var claim =
            jwt.Claims.First(
                x =>
                    x.Type ==
                    ClaimTypes.Name);

        claim.Value
            .Should()
            .Be(usuario.Nome);
    }

    [Fact]
    public void
        GerarToken_DeveConterIssuer()
    {
        var service =
            CriarService();

        var usuario =
            CriarUsuario();

        var token =
            service.GerarToken(usuario);

        var jwt =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

        jwt.Issuer
            .Should()
            .Be(Issuer);
    }

    [Fact]
    public void
        GerarToken_DeveConterAudience()
    {
        var service =
            CriarService();

        var usuario =
            CriarUsuario();

        var token =
            service.GerarToken(usuario);

        var jwt =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

        jwt.Audiences
            .Should()
            .Contain(Audience);
    }

    [Fact]
    public void
        GerarToken_DevePossuirExpiracao()
    {
        var service =
            CriarService();

        var usuario =
            CriarUsuario();

        var antes =
            DateTime.UtcNow.AddHours(1);

        var token =
            service.GerarToken(usuario);

        var depois =
            DateTime.UtcNow.AddHours(1);

        var jwt =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

        jwt.ValidTo
            .Should()
            .BeOnOrAfter(antes.AddSeconds(-2));

        jwt.ValidTo
            .Should()
            .BeOnOrBefore(depois.AddSeconds(2));
    }

    private static TokenService CriarService()
    {
        var settings =
            Options.Create(
                new JwtSettings
                {
                    Key = Key,
                    Issuer = Issuer,
                    Audience = Audience,
                    ExpirationInHours = 1
                });

        return new TokenService(settings);
    }

    private static Usuario CriarUsuario()
    {
        return new Usuario(
            "Usuario Teste",
            $"teste-{Guid.NewGuid():N}@test.com",
            "senha-hash");
    }
}