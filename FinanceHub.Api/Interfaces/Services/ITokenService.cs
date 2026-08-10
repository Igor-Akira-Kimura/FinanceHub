using FinanceHub.Api.Domain.Entities;

namespace FinanceHub.Api.Interfaces.Services
{
    public interface ITokenService
    {
        string GerarToken(Usuario usuario);
    }
}