using FinanceHub.Api.Application.Common;
using System.Security.Claims;

namespace FinanceHub.Api.Interfaces.Services;

public interface ICurrentUserService
{
    CurrentUser Usuario { get; }
}