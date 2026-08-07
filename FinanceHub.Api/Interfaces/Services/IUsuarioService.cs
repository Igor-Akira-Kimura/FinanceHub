using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;

namespace FinanceHub.Api.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<CriarUsuarioResponse> CadastrarAsync(CriarUsuarioRequest request);

        Task<UsuarioResponse> BuscarPorIdAsync(Guid id);

        Task<IEnumerable<UsuarioResponse>> BuscarTodosAsync();

        Task AtualizarAsync(Guid id, AtualizarUsuarioRequest request);
    }
}
