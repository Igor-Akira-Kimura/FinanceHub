using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;

namespace FinanceHub.Api.Interfaces.Services
{
    public interface IAtivoService
    {
        Task<AtivoResponse> CriarAsync(CriarAtivoRequest request);

        Task<AtivoResponse> BuscarPorIdAsync(Guid id);

        Task<IEnumerable<AtivoResponse>> BuscarTodosAsync();

        Task AtualizarAsync(Guid id, AtualizarAtivoRequest request);

        Task DesativarAsync(Guid id);
    }
}
