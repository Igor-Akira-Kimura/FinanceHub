using System.Diagnostics;

namespace FinanceHub.Application.Observability;

public static class FinanceHubActivitySource
{
    public static readonly ActivitySource Instance =
        new("FinanceHub");
}