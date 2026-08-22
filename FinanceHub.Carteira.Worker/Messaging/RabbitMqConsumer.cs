using FinanceHub.Application.Common.Events;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FinanceHub.Carteira.Worker.Messaging;

public class RabbitMqConsumer : IRabbitMqConsumer
{
    private const string ExchangeName =
        "financehub.events";

    private const string MainQueueName =
        "financehub.compra.criada";

    private const string RetryQueueName =
        "financehub.compra.criada.retry";

    private const string DeadLetterQueueName =
        "financehub.compra.criada.dlq";

    private const string MainRoutingKey =
        "compra.criada";

    private const string RetryRoutingKey =
        "compra.criada.retry";

    private const string DeadLetterRoutingKey =
        "compra.criada.dlq";

    private const int MaxRetries = 3;

    private const int RetryDelayMilliseconds = 2000;

    private const string RetryHeader =
        "x-retry-count";

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IConfiguration _configuration;

    public RabbitMqConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName =
                _configuration["RabbitMq:HostName"],

            UserName =
                _configuration["RabbitMq:UserName"],

            Password =
                _configuration["RabbitMq:Password"]
        };

        var connection =
            await factory.CreateConnectionAsync();

        var channel =
            await connection.CreateChannelAsync();

        // =========================================
        // EXCHANGE PRINCIPAL
        // =========================================

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // =========================================
        // QUEUE PRINCIPAL
        // =========================================

        await channel.QueueDeclareAsync(
            queue: MainQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: MainQueueName,
            exchange: ExchangeName,
            routingKey: MainRoutingKey);

        // =========================================
        // QUEUE DE RETRY
        // =========================================

        var retryArguments =
            new Dictionary<string, object?>
            {
                ["x-message-ttl"] =
                    RetryDelayMilliseconds,

                ["x-dead-letter-exchange"] =
                    ExchangeName,

                ["x-dead-letter-routing-key"] =
                    MainRoutingKey
            };

        await channel.QueueDeclareAsync(
            queue: RetryQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: retryArguments);

        await channel.QueueBindAsync(
            queue: RetryQueueName,
            exchange: ExchangeName,
            routingKey: RetryRoutingKey);

        // =========================================
        // DLQ
        // =========================================

        await channel.QueueDeclareAsync(
            queue: DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: DeadLetterQueueName,
            exchange: ExchangeName,
            routingKey: DeadLetterRoutingKey);

        Console.WriteLine(
            "RabbitMQ configurado.");

        Console.WriteLine(
            $"Queue principal: {MainQueueName}");

        Console.WriteLine(
            $"Queue retry: {RetryQueueName}");

        Console.WriteLine(
            $"DLQ: {DeadLetterQueueName}");

        // =========================================
        // CONSUMER
        // =========================================

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync +=
            async (_, eventArgs) =>
            {
                await ProcessarMensagemAsync(
                    channel,
                    eventArgs);
            };

        await channel.BasicConsumeAsync(
            queue: MainQueueName,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(
            Timeout.Infinite,
            cancellationToken);
    }

    private async Task ProcessarMensagemAsync(
        IChannel channel,
        BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body =
                eventArgs.Body.ToArray();

            var message =
                Encoding.UTF8.GetString(body);

            Console.WriteLine(
                $"Mensagem recebida: {message}");

            var evento =
                JsonSerializer.Deserialize<CompraCriadaEvent>(
                    message);

            if (evento is null)
                throw new Exception(
                    "Não foi possível desserializar o evento.");

            if (evento.EventId == Guid.Empty)
                throw new Exception(
                    "EventId inválido.");

            if (evento.CompraId == Guid.Empty)
                throw new Exception(
                    "CompraId inválido.");

            if (evento.CarteiraId == Guid.Empty)
                throw new Exception(
                    "CarteiraId inválido.");

            if (evento.AtivoId == Guid.Empty)
                throw new Exception(
                    "AtivoId inválido.");

            if (evento.Quantidade <= 0)
                throw new Exception(
                    "Quantidade inválida.");

            if (evento.Preco <= 0)
                throw new Exception(
                    "Preço inválido.");

            using var scope =
                _scopeFactory.CreateScope();

            var repository =
                scope.ServiceProvider
                    .GetRequiredService<IProcessedEventRepository>();

            var context =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            // =========================================
            // IDEMPOTÊNCIA
            // =========================================

            var jaProcessado =
                await repository.ExisteAsync(
                    evento.EventId);

            if (jaProcessado)
            {
                Console.WriteLine(
                    $"Evento já processado: {evento.EventId}");

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    false);

                return;
            }

            // =========================================
            // TRANSACTION
            // =========================================

            await using var transaction =
                await context.Database
                    .BeginTransactionAsync();

            try
            {
                Console.WriteLine(
                    $"Processando compra: {evento.CompraId}");

                // =========================================
                // PROCESSAMENTO REAL
                // =========================================

                var processedEvent =
                    new ProcessedEvent(
                        evento.EventId,
                        nameof(CompraCriadaEvent));

                await repository.CriarAsync(
                    processedEvent);

                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                // ACK somente depois do COMMIT
                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    false);

                Console.WriteLine(
                    $"Evento processado: {evento.EventId}");
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Erro ao processar mensagem: {ex.Message}");

            await EnviarParaRetryOuDlqAsync(
                channel,
                eventArgs,
                ex);
        }
    }

    private async Task EnviarParaRetryOuDlqAsync(
        IChannel channel,
        BasicDeliverEventArgs eventArgs,
        Exception exception)
    {
        var retryCount =
            ObterRetryCount(eventArgs.BasicProperties);

        Console.WriteLine(
            $"Tentativa atual: {retryCount}");

        if (retryCount < MaxRetries)
        {
            var novoRetryCount =
                retryCount + 1;

            Console.WriteLine(
                $"Enviando mensagem para retry " +
                $"({novoRetryCount}/{MaxRetries})");

            var properties =
                CriarPropertiesComRetryCount(
                    eventArgs.BasicProperties,
                    novoRetryCount);

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: RetryRoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: eventArgs.Body);

            // A mensagem original foi encaminhada
            // para a fila de retry.
            await channel.BasicAckAsync(
                eventArgs.DeliveryTag,
                false);

            return;
        }

        Console.WriteLine(
            $"Número máximo de retries atingido. " +
            $"Enviando para DLQ.");

        var dlqProperties =
            CriarPropertiesComRetryCount(
                eventArgs.BasicProperties,
                retryCount);

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: DeadLetterRoutingKey,
            mandatory: true,
            basicProperties: dlqProperties,
            body: eventArgs.Body);

        // A mensagem original foi encaminhada
        // para a DLQ.
        await channel.BasicAckAsync(
            eventArgs.DeliveryTag,
            false);
    }

    private static int ObterRetryCount(
        IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is null)
            return 0;

        if (!properties.Headers.TryGetValue(
                RetryHeader,
                out var value))
        {
            return 0;
        }

        return value switch
        {
            int intValue =>
                intValue,

            long longValue =>
                (int)longValue,

            byte byteValue =>
                byteValue,

            byte[] bytes =>
                int.TryParse(
                    Encoding.UTF8.GetString(bytes),
                    out var result)
                    ? result
                    : 0,

            _ => 0
        };
    }

    private static BasicProperties
        CriarPropertiesComRetryCount(
            IReadOnlyBasicProperties originalProperties,
            int retryCount)
    {
        var properties =
            new BasicProperties
            {
                Persistent = true,

                ContentType =
                    originalProperties.ContentType
                    ?? "application/json",

                Headers =
                    originalProperties.Headers is null
                        ? new Dictionary<string, object?>()
                        : new Dictionary<string, object?>(
                            originalProperties.Headers)
            };

        properties.Headers[RetryHeader] =
            retryCount;

        return properties;
    }
}