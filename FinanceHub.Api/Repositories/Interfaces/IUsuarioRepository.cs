using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Api.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task AdicionarAsync(Usuario usuario);
    }
}
