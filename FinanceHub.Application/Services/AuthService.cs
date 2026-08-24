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
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService,
        IPasswordService passwordHasher,
        IValidator<LoginRequest> validator,
        IRefreshTokenService refreshTokenService)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _validator = validator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<LoginResponse> LoginAsync(
    LoginRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);

        var usuario =
            await _usuarioRepository
                .BuscarPorEmailAsync(request.Email);

        if (usuario is null)
            throw new CredenciaisInvalidasException();

        if (!usuario.Ativo)
            throw new CredenciaisInvalidasException();

        if (!_passwordHasher.Verify(
                request.Senha,
                usuario.SenhaHash))
        {
            throw new CredenciaisInvalidasException();
        }

        var accessToken =
            _tokenService.GerarToken(usuario);

        var refreshToken =
            await _refreshTokenService
                .CriarAsync(usuario.Id);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<LoginResponse> RefreshAsync(
        RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(
                request.RefreshToken))
        {
            throw new CredenciaisInvalidasException();
        }

        var refreshToken =
            await _refreshTokenService
                .BuscarValidoAsync(
                    request.RefreshToken);

        if (refreshToken is null)
            throw new CredenciaisInvalidasException();

        var accessToken =
            _tokenService.GerarToken(
                refreshToken.Usuario);

        await _refreshTokenService
            .RevogarAsync(refreshToken);

        var novoRefreshToken =
            await _refreshTokenService
                .CriarAsync(
                    refreshToken.UsuarioId);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = novoRefreshToken
        };
    }
}