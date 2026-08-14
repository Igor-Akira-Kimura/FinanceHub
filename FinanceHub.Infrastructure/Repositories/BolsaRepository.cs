using FinanceHub.Infrastructure.Data;
using FinanceHub.Domain.Entities;
using FinanceHub.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Repositories;

public class BolsaRepository : IBolsaRepository
{
    private readonly AppDbContext _context;

    public BolsaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Bolsa?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Bolsas
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && b.Ativa);
    }

    public async Task<IEnumerable<Bolsa>> BuscarTodasAsync()
    {
        return await _context.Bolsas
            .AsNoTracking()
            .Where(b => b.Ativa)
            .OrderBy(b => b.Nome)
            .ToListAsync();
    }
}