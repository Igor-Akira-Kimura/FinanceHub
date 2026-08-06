using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;

namespace FinanceHub.Api.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<CriarUsuarioResponse> CadastrarAsync(CriarUsuarioRequest request);
    }
}
