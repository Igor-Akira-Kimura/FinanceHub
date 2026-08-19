using FinanceHub.Application.Common.Outbox;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class OutboxRepositoryTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public OutboxRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task CriarAsync_DeveAdicionarMensagem()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new OutboxRepository(context);

        var message = CriarMessage();

        await repository.CriarAsync(message);

        await context.SaveChangesAsync();

        var resultado =
            await context.OutboxMessages
                .FindAsync(message.Id);

        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task BuscarPendentesAsync_DeveRetornarSomenteNaoProcessadas()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var pendente = CriarMessage();
        var processada = CriarMessage();

        processada.ProcessedAt =
            DateTime.UtcNow;

        await context.OutboxMessages.AddRangeAsync(
            pendente,
            processada);

        await context.SaveChangesAsync();

        var repository =
            new OutboxRepository(context);

        var result =
            await repository.BuscarPendentesAsync();

        result.Should().Contain(x => x.Id == pendente.Id);
        result.Should().NotContain(x => x.Id == processada.Id);
    }

    [Fact]
    public async Task BuscarPendentesAsync_DeveOrdenarPorCreatedAt()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var maisAntiga = CriarMessage();
        var maisNova = CriarMessage();

        maisAntiga.CreatedAt =
            DateTime.UtcNow.AddMinutes(-10);

        maisNova.CreatedAt =
            DateTime.UtcNow;

        await context.OutboxMessages.AddRangeAsync(
            maisNova,
            maisAntiga);

        await context.SaveChangesAsync();

        var repository =
            new OutboxRepository(context);

        var result =
            (await repository.BuscarPendentesAsync())
            .ToList();

        result.Should().ContainInOrder(
            maisAntiga,
            maisNova);
    }

    [Fact]
    public async Task MarcarComoProcessadoAsync_MensagemExistente_DevePreencherProcessedAt()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var message = CriarMessage();

        await context.OutboxMessages.AddAsync(message);
        await context.SaveChangesAsync();

        var repository =
            new OutboxRepository(context);

        var processedAt =
            DateTime.UtcNow;

        await repository.MarcarComoProcessadoAsync(
            message.Id,
            processedAt);

        await repository.MarcarComoProcessadoAsync(
            message.Id,
            processedAt);

        await context.SaveChangesAsync();

        message.ProcessedAt
            .Should()
            .Be(processedAt);
    }

    [Fact]
    public async Task MarcarComoProcessadoAsync_MensagemInexistente_NaoDeveFalhar()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new OutboxRepository(context);

        var act = async () =>
            await repository.MarcarComoProcessadoAsync(
                Guid.NewGuid(),
                DateTime.UtcNow);

        await act.Should().NotThrowAsync();
    }

    private static OutboxMessage CriarMessage()
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "TesteEvent",
            Payload = "{}",
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = null
        };
    }
}