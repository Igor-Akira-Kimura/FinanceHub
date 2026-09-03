using FinanceHub.Carteira.Worker.Messaging;

namespace FinanceHub.Carteira.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IMessageConsumer _consumer;

        public Worker(IMessageConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await _consumer.StartAsync(stoppingToken);
        }
    }
}