using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Repositories;

public class RefreshTokenRepository
    : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(
        RefreshToken refreshToken)
    {
        await _context.RefreshTokens
            .AddAsync(refreshToken);

        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> BuscarPorTokenHashAsync(
        string tokenHash)
    {
        return await _context.RefreshTokens
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}