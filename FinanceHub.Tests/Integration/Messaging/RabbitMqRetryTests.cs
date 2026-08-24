using FluentAssertions;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;

namespace FinanceHub.Tests.Integration.Messaging;

public class RabbitMqRetryTests
{
    private const string ExchangeName =
        "financehub.events";

    private const string RoutingKey =
        "compra.criada";

    private const string DeadLetterQueue =
        "financehub.compra.criada.dlq";

    private const string RetryQueue =
    "financehub.compra.criada.retry";

    private const string MainQueue =
        "financehub.compra.criada";

    [Fact]
    public async Task
        MensagemComErro_AposNumeroMaximoDeRetries_DeveIrParaDLQ()
    {
        // Arrange

        var factory = new ConnectionFactory
        {
            HostName =
                Environment.GetEnvironmentVariable(
                    "RABBITMQ_HOST")
                ?? "localhost",

            UserName =
                Environment.GetEnvironmentVariable(
                    "RABBITMQ_USERNAME")
                ?? "guest",

            Password =
                Environment.GetEnvironmentVariable(
                    "RABBITMQ_PASSWORD")
                ?? "guest"
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true);

        await channel.QueueDeclareAsync(
            queue: DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var retryArguments =
            new Dictionary<string, object?>
            {
                ["x-message-ttl"] = 2000,
                ["x-dead-letter-exchange"] = ExchangeName,
                ["x-dead-letter-routing-key"] = RoutingKey
            };

        await channel.QueueDeclareAsync(
            queue: RetryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArguments);

        await channel.QueueDeclareAsync(
            queue: MainQueue,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Mensagem propositalmente inválida.
        // O Consumer não conseguirá desserializar.
        var mensagemInvalida =
            """
            {
                "isso": "não é um CompraCriadaEvent válido"
            }
            """;

        var body =
            Encoding.UTF8.GetBytes(
                mensagemInvalida);

        // Limpa mensagens antigas da DLQ
        while (true)
        {
            var mensagem =
                await channel.BasicGetAsync(
                    DeadLetterQueue,
                    autoAck: true);

            if (mensagem is null)
                break;
        }

        // Act

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            mandatory: true,
            basicProperties: new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            },
            body: body);

        // Espera a mensagem passar pelos retries
        BasicGetResult? mensagemNaDlq = null;

        for (var tentativa = 0;
             tentativa < 20;
             tentativa++)
        {
            await Task.Delay(500);

            mensagemNaDlq =
                await channel.BasicGetAsync(
                    DeadLetterQueue,
                    autoAck: false);

            if (mensagemNaDlq is not null)
                break;
        }

        // Assert

        mensagemNaDlq
            .Should()
            .NotBeNull(
                "a mensagem deveria terminar na DLQ após os retries");

        var mensagemRecebida =
            Encoding.UTF8.GetString(
                mensagemNaDlq!.Body.ToArray());

        mensagemRecebida
            .Should()
            .Contain(
                "não é um CompraCriadaEvent válido");

        await channel.BasicAckAsync(
            mensagemNaDlq.DeliveryTag,
            false);
    }
}