using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task AdicionarAsync(Usuario usuario);

        Task<Usuario?> BuscarPorEmailAsync(string email);

        Task<Usuario?> BuscarPorIdAsync(Guid id);

        Task<IEnumerable<Usuario>> BuscarTodosAsync();

        Task SalvarAlteracoesAsync();
    }
}
