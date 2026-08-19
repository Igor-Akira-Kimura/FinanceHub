using FinanceHub.Application.Requests.Auth;
using FinanceHub.Application.Responses;
using FinanceHub.Domain.Exceptions;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FluentValidation;

namespace FinanceHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordHasher;

    private readonly IValidator<LoginRequest> _validator;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService,
        IPasswordService passwordHasher,
        IValidator<LoginRequest> validator)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);

        var usuario = await _usuarioRepository.BuscarPorEmailAsync(request.Email);

        if (usuario is null)
            throw new CredenciaisInvalidasException();

        if (!usuario.Ativo)
            throw new CredenciaisInvalidasException();

        if (!_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
            throw new CredenciaisInvalidasException();

        var token = _tokenService.GerarToken(usuario);

        return new LoginResponse
        {
            Token = token
        };
    }
}