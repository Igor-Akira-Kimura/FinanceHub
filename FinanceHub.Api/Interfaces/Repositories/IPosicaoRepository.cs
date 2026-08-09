using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Api.Interfaces.Repositories
{
    public interface IPosicaoRepository
    {
        Task CriarAsync(Posicao posicao);

        Task<Posicao?> BuscarPorCarteiraEAtivoAsync(Guid carteiraId, Guid ativoId);

        Task SalvarAlteracoesAsync();

        Task RemoverAsync(Posicao posicao);
    }
}
