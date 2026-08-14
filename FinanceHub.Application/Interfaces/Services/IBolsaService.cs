using FinanceHub.Application.Responses;

namespace FinanceHub.Application.Interfaces.Services
{
    public interface IBolsaService
    {
        Task<IEnumerable<BolsaResponse>> BuscarTodasAsync();
    }
}
