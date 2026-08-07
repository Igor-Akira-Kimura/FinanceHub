using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Api.Interfaces.Repositories
{
    public interface IBolsaRepository
    {
        Task<Bolsa?> BuscarPorIdAsync(Guid id);

        Task<IEnumerable<Bolsa>> BuscarTodasAsync();
    }
}
