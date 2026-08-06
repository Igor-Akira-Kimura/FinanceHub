using FinanceHub.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
}