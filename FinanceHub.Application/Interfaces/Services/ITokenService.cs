using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GerarToken(Usuario usuario);
    }
}