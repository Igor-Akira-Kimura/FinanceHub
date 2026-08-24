using System.Security.Cryptography;
using System.Text;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Domain.Entities;

namespace FinanceHub.Infrastructure.Security;

public class RefreshTokenService
    : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repository;

    public RefreshTokenService(
        IRefreshTokenRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> CriarAsync(
        Guid usuarioId)
    {
        var token =
            GerarToken();

        var tokenHash =
            GerarHash(token);

        var refreshToken =
            new RefreshToken(
                usuarioId,
                tokenHash,
                DateTime.UtcNow.AddDays(30));

        await _repository
            .AdicionarAsync(refreshToken);

        return token;
    }

    public async Task<RefreshToken?> BuscarValidoAsync(
        string refreshToken)
    {
        var tokenHash =
            GerarHash(refreshToken);

        var token =
            await _repository
                .BuscarPorTokenHashAsync(tokenHash);

        if (token is null)
            return null;

        if (!token.IsActive)
            return null;

        if (!token.Usuario.Ativo)
            return null;

        return token;
    }

    public async Task RevogarAsync(
        RefreshToken refreshToken)
    {
        refreshToken.Revogar();

        await _repository
            .SalvarAlteracoesAsync();
    }

    private static string GerarToken()
    {
        var bytes =
            RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    private static string GerarHash(
        string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token)));
    }
}