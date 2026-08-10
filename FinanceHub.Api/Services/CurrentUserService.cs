using FinanceHub.Api.Application.Common;
using FinanceHub.Api.Interfaces.Services;
using System.Security.Claims;

namespace FinanceHub.Api.Services;

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