using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Api.Interfaces.Repositories
{
    public interface ICarteiraRepository
    {
        Task CriarAsync(Carteira carteira);

        Task<Carteira?> BuscarPorIdAsync(Guid id);

        Task<Carteira?> BuscarPorIdComPosicoesAsync(Guid id);

        Task<Carteira?> BuscarPorNomeAsync(Guid usuarioId, string nome);

        Task<IEnumerable<Carteira>> BuscarTodasAsync(Guid usuarioId);

        Task SalvarAlteracoesAsync();
    }
}
