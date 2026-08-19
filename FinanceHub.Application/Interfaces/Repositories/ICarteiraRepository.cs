using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories
{
    public interface ICarteiraRepository
    {
        Task CriarAsync(Carteira carteira);

        Task<Carteira?> BuscarPorIdAsync(Guid id);

        Task<Carteira?> BuscarPorIdComPosicoesAsync(Guid id);

        Task<Carteira?> BuscarPorNomeAsync(Guid usuarioId, string nome);

        Task<IEnumerable<Carteira>> BuscarTodasAsync(Guid usuarioId);

        Task<int> DebitarSaldoAsync(Guid carteiraId, decimal valor);

        Task<int> CreditarSaldoAsync(Guid carteiraId, decimal valor);

        Task SalvarAlteracoesAsync();
    }
}
