using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Responses;

namespace FinanceHub.Application.Interfaces.Services
{
    public interface ICarteiraService
    {
        Task<Guid> CriarAsync(CriarCarteiraRequest request);

        Task<IEnumerable<CarteiraResponse>> BuscarTodasAsync(Guid usuarioId);

        Task<IEnumerable<CarteiraResponse>> BuscarMinhasAsync();

        Task<CarteiraResponse> BuscarPorIdAsync(Guid id);

        Task ComprarAtivoAsync(ComprarAtivoRequest request);

        Task VenderAtivoAsync(VenderAtivoRequest request);
    }
}
