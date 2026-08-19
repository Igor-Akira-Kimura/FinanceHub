using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class ProcessedEventRepositoryTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ProcessedEventRepositoryTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task CriarAsync_DeveAdicionarProcessedEvent()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new ProcessedEventRepository(context);

        var evento =
            new ProcessedEvent(
                Guid.NewGuid(),
                "CompraCriadaEvent");

        await repository.CriarAsync(evento);

        await context.SaveChangesAsync();

        var resultado =
            await context.ProcessedEvents
                .FindAsync(evento.Id);

        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task ExisteAsync_EventoExistente_DeveRetornarTrue()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var eventId = Guid.NewGuid();

        var evento =
            new ProcessedEvent(
                eventId,
                "CompraCriadaEvent");

        await context.ProcessedEvents.AddAsync(evento);
        await context.SaveChangesAsync();

        var repository =
            new ProcessedEventRepository(context);

        var result =
            await repository.ExisteAsync(eventId);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExisteAsync_EventoInexistente_DeveRetornarFalse()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new ProcessedEventRepository(context);

        var result =
            await repository.ExisteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }
}