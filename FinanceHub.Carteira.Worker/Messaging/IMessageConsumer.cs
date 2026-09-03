namespace FinanceHub.Carteira.Worker.Messaging;

public interface IMessageConsumer
{
    Task StartAsync(CancellationToken cancellationToken);
}