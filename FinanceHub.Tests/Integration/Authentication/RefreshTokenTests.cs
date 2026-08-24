using FinanceHub.Application.Requests.Auth;
using FinanceHub.Application.Responses;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace FinanceHub.Tests.Integration.Authentication;

public class RefreshTokenTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;

    public RefreshTokenTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDatabase();

        _factory =
            new CustomWebApplicationFactory(
                _fixture.ConnectionString);
    }

    [Fact]
    public async Task Refresh_TokenValido_DeveRetornarNovosTokens()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                $"refresh-{Guid.NewGuid():N}@test.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "Senha123!"));

        await CriarUsuarioAsync(usuario);

        using var client =
            _factory.CreateClient();

        var loginRequest =
            new LoginRequest
            {
                Email = usuario.Email,
                Senha = "Senha123!"
            };

        var loginResponse =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                loginRequest);

        loginResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        login.Should()
            .NotBeNull();

        login!.RefreshToken
            .Should()
            .NotBeNullOrWhiteSpace();

        var refreshRequest =
            new RefreshTokenRequest
            {
                RefreshToken =
                    login.RefreshToken
            };

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                refreshRequest);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        result.Should()
            .NotBeNull();

        result!.AccessToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RefreshToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RefreshToken
            .Should()
            .NotBe(login.RefreshToken);

        var handler = new JwtSecurityTokenHandler();

        var novoAccessToken =
            handler.ReadJwtToken(
                result.AccessToken);

        novoAccessToken.Claims
            .Should()
            .Contain(x =>
                x.Type ==
                    System.Security.Claims.ClaimTypes.NameIdentifier &&
                x.Value ==
                    usuario.Id.ToString());

        novoAccessToken.Claims
            .Should()
            .Contain(x =>
                x.Type ==
                    System.Security.Claims.ClaimTypes.Email &&
                x.Value ==
                    usuario.Email);

        novoAccessToken.Claims
            .Should()
            .Contain(x =>
                x.Type ==
                    System.Security.Claims.ClaimTypes.Name &&
                x.Value ==
                    usuario.Nome);
    }

    [Fact]
    public async Task Refresh_TokenValido_DeveRevogarTokenAntigo()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                $"rotation-{Guid.NewGuid():N}@test.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "Senha123!"));

        await CriarUsuarioAsync(usuario);

        using var client =
            _factory.CreateClient();

        var loginResponse =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginRequest
                {
                    Email = usuario.Email,
                    Senha = "Senha123!"
                });

        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        login.Should()
            .NotBeNull();

        var antigoRefreshToken =
            login!.RefreshToken;

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                new RefreshTokenRequest
                {
                    RefreshToken =
                        antigoRefreshToken
                });

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        // Assert

        await using var scope =
            _factory.Services
                .CreateAsyncScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        var refreshTokens =
            await context.RefreshTokens
                .Include(x => x.Usuario)
                .Where(x =>
                    x.UsuarioId == usuario.Id)
                .ToListAsync();

        refreshTokens
            .Should()
            .HaveCount(2);

        var tokenAntigo =
            refreshTokens.Single(
                x => x.TokenHash !=
                     refreshTokens
                         .OrderByDescending(
                             x => x.CreatedAt)
                         .First()
                         .TokenHash);

        tokenAntigo.RevokedAt
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task Refresh_TokenAntigoReutilizado_DeveRetornar401()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                $"reuse-{Guid.NewGuid():N}@test.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "Senha123!"));

        await CriarUsuarioAsync(usuario);

        using var client =
            _factory.CreateClient();

        var loginResponse =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginRequest
                {
                    Email = usuario.Email,
                    Senha = "Senha123!"
                });

        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        login.Should()
            .NotBeNull();

        var antigoRefreshToken =
            login!.RefreshToken;

        // Primeiro uso

        var primeiroRefresh =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                new RefreshTokenRequest
                {
                    RefreshToken =
                        antigoRefreshToken
                });

        primeiroRefresh.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        // Segundo uso do mesmo token

        var segundoRefresh =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                new RefreshTokenRequest
                {
                    RefreshToken =
                        antigoRefreshToken
                });

        // Assert

        segundoRefresh.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_TokenInexistente_DeveRetornar401()
    {
        // Arrange

        using var client =
            _factory.CreateClient();

        var request =
            new RefreshTokenRequest
            {
                RefreshToken =
                    "refresh-token-inexistente"
            };

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_TokenVazio_DeveRetornar401()
    {
        // Arrange

        using var client =
            _factory.CreateClient();

        var request =
            new RefreshTokenRequest
            {
                RefreshToken = ""
            };

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_TokenExpirado_DeveRetornar401()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                $"expired-{Guid.NewGuid():N}@test.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "Senha123!"));

        await CriarUsuarioAsync(usuario);

        var tokenOriginal = "token-expirado";

        var tokenHash =
            Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(
                        tokenOriginal)));

        var refreshToken =
            new RefreshToken(
                usuario.Id,
                tokenHash,
                DateTime.UtcNow.AddMinutes(-1));

        await using (
            var scope =
                _factory.Services
                    .CreateAsyncScope())
        {
            var context =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            context.RefreshTokens.Add(
                refreshToken);

            await context.SaveChangesAsync();
        }

        using var client =
            _factory.CreateClient();

        // Não conseguimos usar "hash-expirado" como
        // refresh token real porque o service calcula
        // SHA-256. Portanto usamos um token qualquer:
        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                new RefreshTokenRequest
                {
                    RefreshToken =
                        tokenOriginal
                });

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_UsuarioInativo_DeveRetornar401()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                $"inactive-{Guid.NewGuid():N}@test.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "Senha123!"));

        await CriarUsuarioAsync(usuario);

        using var client =
            _factory.CreateClient();

        var loginResponse =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginRequest
                {
                    Email = usuario.Email,
                    Senha = "Senha123!"
                });

        var login =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>();

        login.Should()
            .NotBeNull();

        // Desativa o usuário depois do login.

        await using (
            var scope =
                _factory.Services
                    .CreateAsyncScope())
        {
            var context =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            var usuarioBanco =
                await context.Usuarios
                    .SingleAsync(x =>
                        x.Id == usuario.Id);

            usuarioBanco.Desativar();

            await context.SaveChangesAsync();
        }

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                new RefreshTokenRequest
                {
                    RefreshToken =
                        login!.RefreshToken
                });

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    private async Task CriarUsuarioAsync(
        Usuario usuario)
    {
        await using var scope =
            _factory.Services
                .CreateAsyncScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();
    }
}