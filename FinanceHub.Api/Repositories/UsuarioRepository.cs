using FinanceHub.Api.Data;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Repositories.Interfaces;

namespace FinanceHub.Api.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);

            await _context.SaveChangesAsync();
        }
    }
}
