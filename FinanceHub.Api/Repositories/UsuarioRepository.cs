using FinanceHub.Api.Data;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> BuscarPorIdAsync(Guid id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<IEnumerable<Usuario>> BuscarTodosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }
    }
}
