using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class UnitOfWorkTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public UnitOfWorkTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task CommitAsync_DevePersistirAlteracoes()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var unitOfWork =
            new UnitOfWork(context);

        var bolsa =
            new Bolsa(
                $"B3-{Guid.NewGuid():N}",
                "Brasil",
                "BRL");

        await context.Bolsas.AddAsync(bolsa);

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.CommitAsync();

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Bolsas
                .FirstOrDefaultAsync(x =>
                    x.Id == bolsa.Id);

        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task RollbackAsync_NaoDevePersistirAlteracoes()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var unitOfWork =
            new UnitOfWork(context);

        var bolsa =
            new Bolsa(
                $"B3-{Guid.NewGuid():N}",
                "Brasil",
                "BRL");

        await context.Bolsas.AddAsync(bolsa);

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.RollbackAsync();

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Bolsas
                .FirstOrDefaultAsync(x =>
                    x.Id == bolsa.Id);

        resultado.Should().BeNull();
    }
}