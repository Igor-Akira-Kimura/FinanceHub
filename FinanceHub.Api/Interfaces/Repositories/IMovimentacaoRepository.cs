using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Api.Interfaces.Repositories;

public interface IMovimentacaoRepository
{
    Task CriarAsync(Movimentacao movimentacao);

    Task<IEnumerable<Movimentacao>> BuscarPorPosicaoAsync(Guid posicaoId);

    Task SalvarAlteracoesAsync();
}