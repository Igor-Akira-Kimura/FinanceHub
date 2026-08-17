using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;

namespace FinanceHub.Infrastructure.Repositories;

public class CompraRepository : ICompraRepository
{
    private readonly AppDbContext _context;

    public CompraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(Compra compra)
    {
        await _context.Compras.AddAsync(compra);
    }
}