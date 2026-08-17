using FinanceHub.Infrastructure.Data;
using FinanceHub.Domain.Entities;
using FinanceHub.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Repositories
{
    public class PosicaoRepository : IPosicaoRepository
    {
        private readonly AppDbContext _dbContext;
        public PosicaoRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CriarAsync(Posicao posicao)
        {
            await _dbContext.Posicoes.AddAsync(posicao);
        }

        public async Task<Posicao?> BuscarPorCarteiraEAtivoAsync(Guid carteiraId, Guid ativoId)
        {
            return await _dbContext.Posicoes.FirstOrDefaultAsync(p => p.CarteiraId == carteiraId && p.AtivoId == ativoId);
        }

        public Task RemoverAsync(Posicao posicao)
        {
            _dbContext.Posicoes.Remove(posicao);

            return Task.CompletedTask;
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
