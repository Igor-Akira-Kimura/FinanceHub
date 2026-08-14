using FinanceHub.Application.Common;
using System.Security.Claims;

namespace FinanceHub.Application.Interfaces.Services;

public interface ICurrentUserService
{
    CurrentUser Usuario { get; }
}