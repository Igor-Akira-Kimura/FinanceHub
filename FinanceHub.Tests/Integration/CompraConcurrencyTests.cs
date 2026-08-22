using FinanceHub.Application.Common.Events;
using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

namespace FinanceHub.Tests.Integration;

public class CompraConcurrencyTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;

    public CompraConcurrencyTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDatabase();

        _factory =
            new CustomWebApplicationFactory(    
                _fixture.ConnectionString);
    }

    [Fact]
    public async Task
        DuasComprasSimultaneas_ComMesmaIdempotencyKey_DeveProcessarApenasUma()
    {
        // Arrange

        var usuario = new Usuario(
            "Usuario Teste",
            $"teste-{Guid.NewGuid():N}@test.com",
            "senha-hash");

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        var carteira = new Carteira(
            "Carteira concorrencia",
            usuario.Id);

        var ativo = new Ativo(
            "PETR4",
            "Petrobras",
            TipoAtivo.Acao,
            bolsa.Id,
            100m);

        await CriarDadosAsync(
            usuario,
            bolsa,
            carteira,
            ativo,
            1000m);

        using var client =
            _factory.CreateClient();

        var token =
            GerarToken(usuario);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var request =
            new ComprarAtivoRequest
            {
                CarteiraId = carteira.Id,
                AtivoId = ativo.Id,
                Quantidade = 5,
                IdempotencyKey =
                    $"CONCORRENCIA-{Guid.NewGuid():N}"
            };

        // Act

        var tarefaA =
            client.PostAsJsonAsync(
                "/api/carteiras/comprar",
                request);

        var tarefaB =
            client.PostAsJsonAsync(
                "/api/carteiras/comprar",
                request);

        var responses =
            await Task.WhenAll(
                tarefaA,
                tarefaB);

        // Assert

        responses.Should().HaveCount(2);

        responses.Should().OnlyContain(
            response =>
                response.StatusCode ==
                HttpStatusCode.NoContent);

        await using var scope =
            _factory.Services
                .CreateAsyncScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        // --------------------------------------------------
        // Compra
        // --------------------------------------------------

        var compras =
            await context.Compras
                .Where(x =>
                    x.IdempotencyKey ==
                    request.IdempotencyKey)
                .ToListAsync();

        compras.Should().HaveCount(1);

        var compra =
            compras.Single();

        compra.CarteiraId
            .Should()
            .Be(carteira.Id);

        compra.AtivoId
            .Should()
            .Be(ativo.Id);

        compra.Quantidade
            .Should()
            .Be(5);

        compra.Preco
            .Should()
            .Be(100m);

        compra.IdempotencyKey
            .Should()
            .Be(request.IdempotencyKey);

        // --------------------------------------------------
        // Posição
        // --------------------------------------------------

        var posicoes =
            await context.Posicoes
                .Where(x =>
                    x.CarteiraId == carteira.Id &&
                    x.AtivoId == ativo.Id)
                .ToListAsync();

        posicoes.Should().HaveCount(1);

        var posicao =
            posicoes.Single();

        posicao.Quantidade
            .Should()
            .Be(5);

        posicao.PrecoMedio
            .Should()
            .Be(100m);

        // --------------------------------------------------
        // Movimentação
        // --------------------------------------------------

        var movimentacoes =
            await context.Movimentacoes
                .Where(x =>
                    x.PosicaoId == posicao.Id)
                .ToListAsync();

        movimentacoes.Should().HaveCount(1);

        // --------------------------------------------------
        // Outbox
        // --------------------------------------------------

        var outboxMessages =
            await context.OutboxMessages
                .Where(x =>
                    x.Type == nameof(CompraCriadaEvent))
                .ToListAsync();

        outboxMessages.Should().HaveCount(1);

        var outboxMessage =
            outboxMessages.Single();

        outboxMessage.Payload
            .Should()
            .Contain(compra.Id.ToString());

        // --------------------------------------------------
        // Saldo
        // --------------------------------------------------

        var carteiraBanco =
            await context.Carteiras
                .SingleAsync(x =>
                    x.Id == carteira.Id);

        carteiraBanco.Saldo
            .Should()
            .Be(500m);
    }

    private async Task CriarDadosAsync(
        Usuario usuario,
        Bolsa bolsa,
        Carteira carteira,
        Ativo ativo,
        decimal saldo)
    {
        await using var scope =
            _factory.Services
                .CreateAsyncScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        context.Usuarios.Add(usuario);
        context.Bolsas.Add(bolsa);
        context.Carteiras.Add(carteira);
        context.Ativos.Add(ativo);

        await context.SaveChangesAsync();

        await context.Carteiras
            .Where(x => x.Id == carteira.Id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    x => x.Saldo,
                    saldo));
    }

    private static string GerarToken(
        Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.Nome),

            new Claim(
                ClaimTypes.Email,
                usuario.Email)
        };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "SuaChaveSuperSecretaComNoMinimo32Caracteres123!"));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: "FinanceHub.Api",
                audience: "FinanceHub.Client",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}