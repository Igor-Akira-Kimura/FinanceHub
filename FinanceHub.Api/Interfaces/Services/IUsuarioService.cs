using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;

namespace FinanceHub.Api.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<CriarUsuarioResponse> CadastrarAsync(CriarUsuarioRequest request);
    }
}
