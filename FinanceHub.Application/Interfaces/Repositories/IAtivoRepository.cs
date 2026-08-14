using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories
{
    public interface IAtivoRepository
    {
        Task CriarAsync(Ativo ativo);

        Task<Ativo?> BuscarPorIdAsync(Guid id);

        Task<Ativo?> BuscarPorIdLeituraAsync(Guid id);

        Task<Ativo?> BuscarPorTickerAsync(string ticker);

        Task<IEnumerable<Ativo>> BuscarTodosAsync();

        Task SalvarAlteracoesAsync();
    }
}
