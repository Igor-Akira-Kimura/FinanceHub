using FinanceHub.Api;
using FinanceHub.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceHub.Tests.Integration;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(
        string connectionString)
    {
        _connectionString = connectionString;

        Environment.SetEnvironmentVariable(
            "Redis__ConnectionString",
            "localhost:6379");
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(
                options =>
                    options.UseSqlServer(
                        _connectionString));
        });
    }
}