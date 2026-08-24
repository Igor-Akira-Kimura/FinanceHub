using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AdicionarAsync(RefreshToken refreshToken);

    Task<RefreshToken?> BuscarPorTokenHashAsync(
        string tokenHash);

    Task SalvarAlteracoesAsync();
}