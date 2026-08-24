using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using FinanceHub.Application.Requests.Auth;
using FinanceHub.Application.Responses;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceHub.Tests.Integration.Authentication;

public class AuthenticationTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;

    public AuthenticationTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDatabase();

        _factory =
            new CustomWebApplicationFactory(
                _fixture.ConnectionString);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornar200EToken()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "igor@login.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "Senha123!"));

        await CriarUsuarioAsync(usuario);

        using var client =
            _factory.CreateClient();

        var request =
            new LoginRequest
            {
                Email = usuario.Email,
                Senha = "Senha123!"
            };

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        result.Should().NotBeNull();

        result!.AccessToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RefreshToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.AccessToken
            .Should()
            .NotBe(result.RefreshToken);
    }

    [Fact]
    public async Task Login_UsuarioNaoEncontrado_DeveRetornar401()
    {
        using var client =
            _factory.CreateClient();

        var request =
            new LoginRequest
            {
                Email = "naoexiste@login.com",
                Senha = "Senha123!"
            };

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_SenhaIncorreta_DeveRetornar401()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "igor-senha@login.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "SenhaCorreta123!"));

        await CriarUsuarioAsync(usuario);

        using var client =
            _factory.CreateClient();

        var request =
            new LoginRequest
            {
                Email = usuario.Email,
                Senha = "SenhaErrada123!"
            };

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        // Assert

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_EmailInvalido_DeveRetornar400()
    {
        using var client =
            _factory.CreateClient();

        var request =
            new LoginRequest
            {
                Email = "email-invalido",
                Senha = "Senha123!"
            };

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_EmailVazio_DeveRetornar400()
    {
        using var client =
            _factory.CreateClient();

        var request =
            new LoginRequest
            {
                Email = "",
                Senha = "Senha123!"
            };

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_SenhaVazia_DeveRetornar400()
    {
        using var client =
            _factory.CreateClient();

        var request =
            new LoginRequest
            {
                Email = "teste@login.com",
                Senha = ""
            };

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornarJwtValido()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "igor-jwt@login.com",
                BCrypt.Net.BCrypt.HashPassword(
                    "Senha123!"));

        await CriarUsuarioAsync(usuario);

        using var client =
            _factory.CreateClient();

        var request =
            new LoginRequest
            {
                Email = usuario.Email,
                Senha = "Senha123!"
            };

        // Act

        var response =
            await client.PostAsJsonAsync(
                "/api/Auth/login",
                request);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        result.Should().NotBeNull();

        result!.AccessToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.RefreshToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.AccessToken
            .Should()
            .NotBe(result.RefreshToken);

        var handler =
            new JwtSecurityTokenHandler();

        var token =
            handler.ReadJwtToken(
                result.AccessToken);

        // Assert

        token.Claims
            .Should()
            .Contain(x =>
                x.Type ==
                    System.Security.Claims.ClaimTypes.NameIdentifier &&
                x.Value ==
                    usuario.Id.ToString());

        token.Claims
            .Should()
            .Contain(x =>
                x.Type ==
                    System.Security.Claims.ClaimTypes.Email &&
                x.Value ==
                    usuario.Email);

        token.Claims
            .Should()
            .Contain(x =>
                x.Type ==
                    System.Security.Claims.ClaimTypes.Name &&
                x.Value ==
                    usuario.Nome);

        token.Claims
            .Should()
            .Contain(x =>
                x.Type ==
                    System.Security.Claims.ClaimTypes.Role &&
                x.Value ==
                    usuario.Role.ToString());

        token.Claims
            .Should()
            .Contain(x =>
                x.Type == "permission" &&
                x.Value == "ComprarAtivos");
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