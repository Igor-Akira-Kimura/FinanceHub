namespace FinanceHub.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }

    public Guid UsuarioId { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    public string TokenHash { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public bool IsRevoked =>
        RevokedAt.HasValue;

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAt;

    public bool IsActive =>
        !IsRevoked &&
        !IsExpired;

    public RefreshToken(
        Guid usuarioId,
        string tokenHash,
        DateTime expiresAt)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException(
                "Usuário é obrigatório.",
                nameof(usuarioId));

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException(
                "Token hash é obrigatório.",
                nameof(tokenHash));

        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        TokenHash = tokenHash;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public void Revogar()
    {
        if (IsRevoked)
            return;

        RevokedAt = DateTime.UtcNow;
    }
}