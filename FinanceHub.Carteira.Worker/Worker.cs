using FinanceHub.Carteira.Worker.Messaging;

namespace FinanceHub.Carteira.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IRabbitMqConsumer _consumer;

        public Worker(IRabbitMqConsumer consumer)
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
