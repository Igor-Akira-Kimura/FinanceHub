using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Authentication;
using FinanceHub.Infrastructure.Configurations;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace FinanceHub.Tests.Unit.Authentication;

public class TokenServiceTests
{
    private readonly JwtSettings _settings;

    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _settings = new JwtSettings
        {
            Key =
                "SuaChaveSuperSecretaComNoMinimo32Caracteres123!",

            Issuer = "FinanceHub.Api",

            Audience = "FinanceHub.Client",

            ExpirationInHours = 1
        };

        var options =
            Options.Create(_settings);

        _tokenService =
            new TokenService(options);
    }

    [Fact]
    public void GerarToken_DeveConterNameIdentifier()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        var claim =
            token.Claims.Single(
                x =>
                    x.Type ==
                    ClaimTypes.NameIdentifier);

        claim.Value
            .Should()
            .Be(usuario.Id.ToString());
    }

    [Fact]
    public void GerarToken_DeveConterEmail()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        var claim =
            token.Claims.Single(
                x =>
                    x.Type ==
                    ClaimTypes.Email);

        claim.Value
            .Should()
            .Be(usuario.Email);
    }

    [Fact]
    public void GerarToken_DeveConterNome()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        var claim =
            token.Claims.Single(
                x =>
                    x.Type ==
                    ClaimTypes.Name);

        claim.Value
            .Should()
            .Be(usuario.Nome);
    }

    [Fact]
    public void GerarToken_DeveConterRole()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        var claim =
            token.Claims.Single(
                x =>
                    x.Type ==
                    ClaimTypes.Role);

        claim.Value
            .Should()
            .Be(UsuarioRole.User.ToString());
    }

    [Fact]
    public void GerarToken_User_DeveConterPermissaoDeComprarAtivos()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        token.Claims
            .Should()
            .Contain(
                x =>
                    x.Type == "permission" &&
                    x.Value ==
                    UsuarioPermission.ComprarAtivos.ToString());
    }

    [Fact]
    public void GerarToken_User_NaoDeveConterPermissaoDeAdministrarUsuarios()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        token.Claims
            .Should()
            .NotContain(
                x =>
                    x.Type == "permission" &&
                    x.Value ==
                    UsuarioPermission.AdministrarUsuarios.ToString());
    }

    [Fact]
    public void GerarToken_DeveConterIssuerCorreto()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        token.Issuer
            .Should()
            .Be(_settings.Issuer);
    }

    [Fact]
    public void GerarToken_DeveConterAudienceCorreta()
    {
        var usuario =
            CriarUsuario();

        var token =
            GerarToken(usuario);

        token.Audiences
            .Should()
            .Contain(_settings.Audience);
    }

    [Fact]
    public void GerarToken_DevePossuirDataDeExpiracao()
    {
        var usuario =
            CriarUsuario();

        var antes =
            DateTime.UtcNow;

        var token =
            GerarToken(usuario);

        var depois =
            DateTime.UtcNow.AddHours(
                _settings.ExpirationInHours);

        token.ValidTo
            .Should()
            .BeAfter(antes);

        token.ValidTo
            .Should()
            .BeBefore(depois.AddSeconds(1));
    }

    private Usuario CriarUsuario()
    {
        return new Usuario(
            "Usuario Teste",
            $"teste-{Guid.NewGuid():N}@test.com",
            "senha-hash");
    }

    private JwtSecurityToken GerarToken(
        Usuario usuario)
    {
        var tokenString =
            _tokenService.GerarToken(usuario);

        return new JwtSecurityTokenHandler()
            .ReadJwtToken(tokenString);
    }
}