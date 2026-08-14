using FinanceHub.Infrastructure.Data;
using FinanceHub.Domain.Entities;
using FinanceHub.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Repositories
{
    public class CarteiraRepository : ICarteiraRepository
    {
        private readonly AppDbContext _context;

        public CarteiraRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CriarAsync(Carteira carteira)
        {
            await _context.Carteiras.AddAsync(carteira);
        }

        public async Task<Carteira?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Carteiras
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Ativa);
        }

        public async Task<Carteira?> BuscarPorIdComPosicoesAsync(Guid id)
        {
            return await _context.Carteiras
                .Include(c => c.Posicoes)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.Ativa);
        }

        public async Task<Carteira?> BuscarPorNomeAsync(Guid usuarioId, string nome)
        {
            return await _context.Carteiras
                .FirstOrDefaultAsync(c =>
                    c.UsuarioId == usuarioId &&
                    c.Nome == nome &&
                    c.Ativa);
        }

        public async Task<IEnumerable<Carteira>> BuscarTodasAsync(Guid usuarioId)
        {
            return await _context.Carteiras
                .AsNoTracking()
                .Where(c =>
                    c.UsuarioId == usuarioId &&
                    c.Ativa)
                .ToListAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
