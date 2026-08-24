using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Configurations;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinanceHub.Tests.Integration.Authentication;

public class AuthorizationTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;

    public AuthorizationTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDatabase();

        _factory =
            new CustomWebApplicationFactory(
                _fixture.ConnectionString);
    }

    [Fact]
    public async Task User_ComPermissaoDeComprar_DeveSerAutorizado()
    {
        using var client =
            _factory.CreateClient();

        var token =
            GerarToken(
                UsuarioRole.User,
                UsuarioPermission.ComprarAtivos);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await client.PostAsJsonAsync(
                "/api/carteiras/comprar",
                CriarRequest());

        response.StatusCode
            .Should()
            .NotBe(HttpStatusCode.Unauthorized);

        response.StatusCode
            .Should()
            .NotBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task User_SemPermissaoDeComprar_DeveRetornar403()
    {
        using var client =
            _factory.CreateClient();

        var token =
            GerarToken(
                UsuarioRole.User);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await client.PostAsJsonAsync(
                "/api/carteiras/comprar",
                CriarRequest());

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SemToken_DeveRetornar401()
    {
        using var client =
            _factory.CreateClient();

        var response =
            await client.PostAsJsonAsync(
                "/api/carteiras/comprar",
                CriarRequest());

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    private ComprarAtivoRequest CriarRequest()
    {
        return new ComprarAtivoRequest
        {
            CarteiraId = Guid.NewGuid(),
            AtivoId = Guid.NewGuid(),
            Quantidade = 1,
            IdempotencyKey =
                Guid.NewGuid().ToString()
        };
    }

    private string GerarToken(
        UsuarioRole role,
        params UsuarioPermission[] permissions)
    {
        var settings =
            _factory.Services
                .GetRequiredService<
                    IOptions<JwtSettings>>()
                .Value;

        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    Guid.NewGuid().ToString()),

                new(
                    ClaimTypes.Name,
                    "Teste"),

                new(
                    ClaimTypes.Email,
                    "teste@test.com"),

                new(
                    ClaimTypes.Role,
                    role.ToString())
            };

        foreach (var permission in permissions)
        {
            claims.Add(
                new Claim(
                    "permission",
                    permission.ToString()));
        }

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    settings.Key));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires:
                    DateTime.UtcNow.AddHours(1),
                signingCredentials:
                    credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}