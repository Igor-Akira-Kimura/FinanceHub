using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Repositories;

public class ProcessedEventRepository
    : IProcessedEventRepository
{
    private readonly AppDbContext _context;

    public ProcessedEventRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExisteAsync(Guid eventId)
    {
        return await _context.ProcessedEvents
            .AnyAsync(x => x.EventId == eventId);
    }

    public async Task CriarAsync(
        ProcessedEvent processedEvent)
    {
        await _context.ProcessedEvents
            .AddAsync(processedEvent);
    }
}