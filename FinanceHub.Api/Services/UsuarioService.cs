using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Repositories.Interfaces;
using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;
using FinanceHub.Api.Services.Interfaces;

namespace FinanceHub.Api.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<CriarUsuarioResponse> CadastrarAsync(CriarUsuarioRequest request)
        {
            var usuario = new Usuario(request.Nome, request.Email, request.Senha);

            await _repository.AdicionarAsync(usuario);

            return new CriarUsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }
    }
}
