using FinanceHub.Api.Responses;

namespace FinanceHub.Api.Interfaces.Services
{
    public interface IBolsaService
    {
        Task<IEnumerable<BolsaResponse>> BuscarTodasAsync();
    }
}
