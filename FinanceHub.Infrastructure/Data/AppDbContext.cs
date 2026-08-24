using FinanceHub.Application.Common.Outbox;
using FinanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Data;

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

    public DbSet<Carteira> Carteiras { get; set; }

    public DbSet<Posicao> Posicoes { get; set; }

    public DbSet<Movimentacao> Movimentacoes { get; set; }

    public DbSet<Compra> Compras { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}