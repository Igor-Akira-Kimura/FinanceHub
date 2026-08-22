using FinanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    public string ConnectionString { get; }

    public DbContextOptions<AppDbContext> Options { get; }

    public DatabaseFixture()
    {
        ConnectionString =
            Environment.GetEnvironmentVariable(
                "TEST_DATABASE_CONNECTION_STRING")
            ?? "Server=(localdb)\\MSSQLLocalDB;" +
               "Database=FinanceHubTestDb;" +
               "Trusted_Connection=True;" +
               "TrustServerCertificate=True;";

        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;
    }

    public async Task InitializeAsync()
    {
        await using var context =
            new AppDbContext(Options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public void ResetDatabase()
    {
        using var context =
            new AppDbContext(Options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}