using FinanceHub.Application.Requests;
using FinanceHub.Application.Responses;

namespace FinanceHub.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<CriarUsuarioResponse> CadastrarAsync(CriarUsuarioRequest request);

        Task<UsuarioResponse> BuscarPorIdAsync(Guid id);

        Task<IEnumerable<UsuarioResponse>> BuscarTodosAsync();

        Task AtualizarAsync(Guid id, AtualizarUsuarioRequest request);

        Task DesativarAsync(Guid id);
    }
}
