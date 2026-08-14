namespace FinanceHub.Application.Common;

public class CurrentUser
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;
}