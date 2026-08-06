using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Repositories.Interfaces;

namespace FinanceHub.Api.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public Task AdicionarAsync(Usuario usuario)
        {
            return Task.CompletedTask;
        }
    }
}
