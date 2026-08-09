using FinanceHub.Api.Data;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Api.Repositories
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

        public async Task SalvarAlteracoesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task RemoverAsync(Posicao posicao)
        {
            _dbContext.Posicoes.Remove(posicao);

            return Task.CompletedTask;
        }
    }
}
