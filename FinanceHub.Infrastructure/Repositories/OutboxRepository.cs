using FinanceHub.Application.Common.Outbox;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _context;

    public OutboxRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(OutboxMessage message)
    {
        await _context.OutboxMessages.AddAsync(message);
    }

    public async Task<IEnumerable<OutboxMessage>> BuscarPendentesAsync()
    {
        return await _context.OutboxMessages
            .Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task MarcarComoProcessadoAsync(
        Guid id,
        DateTime processedAt)
    {
        var message = await _context.OutboxMessages
            .FirstOrDefaultAsync(x => x.Id == id);

        if (message is null)
            return;

        message.ProcessedAt = processedAt;
    }
}