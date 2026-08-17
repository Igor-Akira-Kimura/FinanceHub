using FinanceHub.Application.Common.Outbox;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Infrastructure.Data;

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
}