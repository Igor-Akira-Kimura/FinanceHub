using FinanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    private const string ConnectionString =
        "Server=localhost,1433;" +
        "Database=FinanceHubTestDb;" +
        "User Id=sa;" +
        "Password=FinanceHub@123;" +
        "TrustServerCertificate=True;";

    public DbContextOptions<AppDbContext> Options { get; }

    public DatabaseFixture()
    {
        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
    }

    public async Task InitializeAsync()
    {
        await using var context = new AppDbContext(Options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public void ResetDatabase()
    {
        using var context = new AppDbContext(Options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}