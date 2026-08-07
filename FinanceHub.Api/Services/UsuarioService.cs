using Azure.Core;
using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.Interfaces.Repositories;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;
using FluentValidation;

namespace FinanceHub.Api.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IValidator<CriarUsuarioRequest> _validator;
        private readonly IValidator<AtualizarUsuarioRequest> _atualizarValidator;

        public UsuarioService(
            IUsuarioRepository repository,
            IValidator<CriarUsuarioRequest> validator,
            IValidator<AtualizarUsuarioRequest> atualizarValidator)  
        {
            _repository = repository;
            _validator = validator;
            _atualizarValidator = atualizarValidator;
        }

        public async Task<CriarUsuarioResponse> CadastrarAsync(CriarUsuarioRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var usuario = new Usuario(request.Nome, request.Email, request.Senha);

            var usuarioExistente = await _repository.BuscarPorEmailAsync(usuario.Email);

            if (usuarioExistente != null)
            {
                throw new EmailJaCadastradoException(usuario.Email);
            }

            await _repository.AdicionarAsync(usuario);

            return new CriarUsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }

        public async Task<UsuarioResponse> BuscarPorIdAsync(Guid id)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);

            if (usuario is null)
            {
                throw new UsuarioNaoEncontradoException(id);
            }

            return new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };
        }

        public async Task<IEnumerable<UsuarioResponse>> BuscarTodosAsync()
        {
            var usuarios = await _repository.BuscarTodosAsync();

            return usuarios.Select(usuario => new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            });
        }

        public async Task AtualizarAsync(Guid id, AtualizarUsuarioRequest request)
        {
            await _atualizarValidator.ValidateAndThrowAsync(request);

            var usuario = await _repository.BuscarPorIdAsync(id);

            if (usuario is null)
            {
                throw new UsuarioNaoEncontradoException(id);
            }

            var usuarioComMesmoEmail = await _repository.BuscarPorEmailAsync(request.Email);

            if (usuarioComMesmoEmail is not null && usuarioComMesmoEmail.Id != usuario.Id)
            {
                throw new EmailJaCadastradoException(request.Email);
            }

            usuario.Atualizar(request.Nome, request.Email);

            await _repository.SalvarAlteracoesAsync();
        }

        public async Task DesativarAsync(Guid id)
        {
            var usuario = await _repository.BuscarPorIdAsync(id);

            if (usuario is null)
            {
                throw new UsuarioNaoEncontradoException(id);
            }

            usuario.Desativar();

            await _repository.SalvarAlteracoesAsync();
        }
    }
}
