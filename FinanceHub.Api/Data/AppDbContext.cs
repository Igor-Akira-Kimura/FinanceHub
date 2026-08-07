using FinanceHub.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Bolsa> Bolsas { get; set; }

    public DbSet<Ativo> Ativos { get; set; }

}