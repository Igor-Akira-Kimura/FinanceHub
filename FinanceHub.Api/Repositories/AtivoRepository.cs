using FinanceHub.Api.Data;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Api.Repositories;

public class AtivoRepository : IAtivoRepository
{
    private readonly AppDbContext _context;

    public AtivoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(Ativo ativo)
    {
        await _context.Ativos.AddAsync(ativo);
    }

    public async Task<Ativo?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Ativos
            .Include(a => a.Bolsa)
            .FirstOrDefaultAsync(a => a.Id == id && a.EstaAtivo);
    }

    public async Task<Ativo?> BuscarPorIdLeituraAsync(Guid id)
    {
        return await _context.Ativos
            .AsNoTracking()
            .Include(a => a.Bolsa)
            .FirstOrDefaultAsync(a => a.Id == id && a.EstaAtivo);
    }

    public async Task<Ativo?> BuscarPorTickerAsync(string ticker)
    {
        return await _context.Ativos
            .FirstOrDefaultAsync(a => a.Ticker == ticker && a.EstaAtivo);
    }

    public async Task<IEnumerable<Ativo>> BuscarTodosAsync()
    {
        return await _context.Ativos
            .AsNoTracking()
            .Include(a => a.Bolsa)
            .Where(a => a.EstaAtivo)
            .ToListAsync();
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}