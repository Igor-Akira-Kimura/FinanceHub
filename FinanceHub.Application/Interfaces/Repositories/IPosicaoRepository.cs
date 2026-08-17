using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories
{
    public interface IPosicaoRepository
    {
        Task CriarAsync(Posicao posicao);

        Task<Posicao?> BuscarPorCarteiraEAtivoAsync(Guid carteiraId, Guid ativoId);

        Task RemoverAsync(Posicao posicao);

        Task SalvarAlteracoesAsync();
    }
}
