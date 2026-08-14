using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories;

public interface IMovimentacaoRepository
{
    Task CriarAsync(Movimentacao movimentacao);

    Task<IEnumerable<Movimentacao>> BuscarPorPosicaoAsync(Guid posicaoId);

    Task SalvarAlteracoesAsync();
}