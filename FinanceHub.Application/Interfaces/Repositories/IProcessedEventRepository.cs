using FinanceHub.Domain.Entities;

namespace FinanceHub.Application.Interfaces.Repositories;

public interface IProcessedEventRepository
{
    Task<bool> ExisteAsync(Guid eventId);

    Task CriarAsync(ProcessedEvent processedEvent);
}