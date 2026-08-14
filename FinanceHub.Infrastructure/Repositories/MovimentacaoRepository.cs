using FinanceHub.Infrastructure.Data;
using FinanceHub.Domain.Entities;
using FinanceHub.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Repositories;

public class MovimentacaoRepository : IMovimentacaoRepository
{
    private readonly AppDbContext _context;

    public MovimentacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(Movimentacao movimentacao)
    {
        await _context.Movimentacoes.AddAsync(movimentacao);
    }

    public async Task<IEnumerable<Movimentacao>> BuscarPorPosicaoAsync(Guid posicaoId)
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .Where(m => m.PosicaoId == posicaoId)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToListAsync();
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}