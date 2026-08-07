using FinanceHub.Api.Data;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Api.Repositories;

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