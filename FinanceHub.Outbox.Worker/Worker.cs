using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Outbox.Worker.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Outbox.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventPublisher _eventPublisher;

    public Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory,
        IEventPublisher eventPublisher)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _eventPublisher = eventPublisher;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var outboxRepository =
                scope.ServiceProvider
                    .GetRequiredService<IOutboxRepository>();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            var mensagens =
                await outboxRepository.BuscarPendentesAsync();

            foreach (var mensagem in mensagens)
            {
                try
                {
                    _logger.LogInformation(
                        "Publicando evento {Type} - {Id}",
                        mensagem.Type,
                        mensagem.Id);

                    await _eventPublisher.PublishAsync(
                        mensagem.Type,
                        mensagem.Payload,
                        stoppingToken);

                    await outboxRepository
                        .MarcarComoProcessadoAsync(
                            mensagem.Id,
                            DateTime.UtcNow);

                    await dbContext.SaveChangesAsync(
                        stoppingToken);

                    _logger.LogInformation(
                        "Evento {Id} processado com sucesso.",
                        mensagem.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Erro ao processar evento {Id}.",
                        mensagem.Id);
                }
            }

            await Task.Delay(
                TimeSpan.FromSeconds(5),
                stoppingToken);
        }
    }
}