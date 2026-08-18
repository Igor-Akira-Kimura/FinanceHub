using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Responses;
using FluentValidation;

namespace FinanceHub.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IValidator<CriarUsuarioRequest> _validator;
        private readonly IValidator<AtualizarUsuarioRequest> _atualizarValidator;
        private readonly IPasswordService _passwordHasher;

        public UsuarioService(
            IUsuarioRepository repository,
            IValidator<CriarUsuarioRequest> validator,
            IValidator<AtualizarUsuarioRequest> atualizarValidator,
            IPasswordService passwordHasher)  
        {
            _repository = repository;
            _validator = validator;
            _atualizarValidator = atualizarValidator;
            _passwordHasher = passwordHasher;
        }

        public async Task<CriarUsuarioResponse> CadastrarAsync(CriarUsuarioRequest request)
        {
            await _validator.ValidateAndThrowAsync(request);

            var usuarioExistente = await _repository.BuscarPorEmailAsync(request.Email);

            if (usuarioExistente is not null)
            {
                throw new EmailJaCadastradoException(request.Email);
            }

            var senhaHash = _passwordHasher.Hash(request.Senha);

            var usuario = new Usuario(request.Nome, request.Email, senhaHash);

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
