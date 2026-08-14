using FinanceHub.Application.Requests.Auth;
using FinanceHub.Application.Responses;

namespace FinanceHub.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}