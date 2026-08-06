using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Api.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task AdicionarAsync(Usuario usuario);

        Task<Usuario?> BuscarPorEmailAsync(string email);
    }
}
