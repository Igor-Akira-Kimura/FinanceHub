using RabbitMQ.Client;
using System.Text;

namespace FinanceHub.Outbox.Worker.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqEventPublisher(
        IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName =
                configuration["RabbitMq:HostName"]
                ?? throw new InvalidOperationException(
                    "RabbitMq:HostName não configurado."),

            UserName =
                configuration["RabbitMq:UserName"]
                ?? throw new InvalidOperationException(
                    "RabbitMq:UserName não configurado."),

            Password =
                configuration["RabbitMq:Password"]
                ?? throw new InvalidOperationException(
                    "RabbitMq:Password não configurado.")
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
        var body =
            Encoding.UTF8.GetBytes(payload);

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