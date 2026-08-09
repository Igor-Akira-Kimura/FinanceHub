using FinanceHub.Api.Application.Requests.Carteiras;
using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;

namespace FinanceHub.Api.Interfaces.Services
{
    public interface ICarteiraService
    {
        Task<Guid> CriarAsync(CriarCarteiraRequest request);

        Task<IEnumerable<CarteiraResponse>> BuscarTodasAsync(Guid usuarioId);

        Task<CarteiraResponse> BuscarPorIdAsync(Guid id);

        Task ComprarAtivoAsync(ComprarAtivoRequest request);

        Task VenderAtivoAsync(VenderAtivoRequest request);
    }
}
