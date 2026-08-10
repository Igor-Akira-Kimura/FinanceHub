using FinanceHub.Api.Application.Requests.Auth;
using FinanceHub.Api.Application.Responses;

namespace FinanceHub.Api.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}