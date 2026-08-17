using RabbitMQ.Client;
using System.Text;

namespace FinanceHub.Outbox.Worker.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqEventPublisher()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnectionAsync()
            .GetAwaiter()
            .GetResult();

        _channel = _connection.CreateChannelAsync()
            .GetAwaiter()
            .GetResult();
    }

    public async Task PublishAsync(
        string type,
        string payload,
        CancellationToken cancellationToken)
    {
        await _channel.ExchangeDeclareAsync(
            exchange: "financehub.events",
            type: ExchangeType.Direct,
            durable: true);

        await _channel.QueueDeclareAsync(
            queue: "financehub.compra.criada",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue: "financehub.compra.criada",
            exchange: "financehub.events",
            routingKey: "compra.criada");

        var body = Encoding.UTF8.GetBytes(payload);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await _channel.BasicPublishAsync(
            exchange: "financehub.events",
            routingKey: "compra.criada",
            mandatory: true,
            basicProperties: properties,
            body: body);
    }
}