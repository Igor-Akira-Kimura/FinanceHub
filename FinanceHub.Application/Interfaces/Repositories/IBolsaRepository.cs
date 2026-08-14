using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories
{
    public interface IBolsaRepository
    {
        Task<Bolsa?> BuscarPorIdAsync(Guid id);

        Task<IEnumerable<Bolsa>> BuscarTodasAsync();
    }
}
