using FinanceHub.Application.Common;
using FinanceHub.Application.Interfaces.Services;
using System.Security.Claims;

namespace FinanceHub.Api.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser Usuario =>
    new CurrentUser
    {
        Id = Guid.Parse(
            _httpContextAccessor.HttpContext!
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier)!),

        Nome = _httpContextAccessor.HttpContext!
            .User
            .FindFirstValue(ClaimTypes.Name)!,

        Email = _httpContextAccessor.HttpContext!
            .User
            .FindFirstValue(ClaimTypes.Email)!
    };
}