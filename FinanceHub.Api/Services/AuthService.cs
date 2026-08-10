using FinanceHub.Api.Application.Requests.Auth;
using FinanceHub.Api.Application.Responses;
using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.Exceptions;
using FinanceHub.Api.Interfaces.Repositories;
using FinanceHub.Api.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace FinanceHub.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordHasher;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService,
        IPasswordService passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.BuscarPorEmailAsync(request.Email);

        if (usuario is null)
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