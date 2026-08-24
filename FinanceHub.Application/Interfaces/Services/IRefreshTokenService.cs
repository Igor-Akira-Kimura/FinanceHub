using FinanceHub.Domain.Entities;

public interface IRefreshTokenService
{
    Task<string> CriarAsync(Guid usuarioId);

    Task<RefreshToken?> BuscarValidoAsync(
        string refreshToken);

    Task RevogarAsync(
        RefreshToken refreshToken);
}