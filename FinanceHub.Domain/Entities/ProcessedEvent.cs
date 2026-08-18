namespace FinanceHub.Domain.Entities;

public class ProcessedEvent
{
    public Guid Id { get; private set; }

    public Guid EventId { get; private set; }

    public string EventType { get; private set; } = null!;

    public DateTime ProcessedAt { get; private set; }

    private ProcessedEvent()
    {
    }

    public ProcessedEvent(
        Guid eventId,
        string eventType)
    {
        if (eventId == Guid.Empty)
            throw new ArgumentException(
                "EventId inválido.",
                nameof(eventId));

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException(
                "O tipo do evento é obrigatório.",
                nameof(eventType));

        Id = Guid.NewGuid();
        EventId = eventId;
        EventType = eventType;
        ProcessedAt = DateTime.UtcNow;
    }
}