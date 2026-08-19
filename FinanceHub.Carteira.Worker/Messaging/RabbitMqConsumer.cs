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
    private readonly IServiceScopeFactory _scopeFactory;
    
    private readonly IConfiguration _configuration;

    public RabbitMqConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"],
            UserName = _configuration["RabbitMq:UserName"],
            Password = _configuration["RabbitMq:Password"]
        };

        var connection =
            await factory.CreateConnectionAsync();

        var channel =
            await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "financehub.compra.criada",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        //consumer.ReceivedAsync += async (_, eventArgs) =>
        //{
        //    var body = eventArgs.Body.ToArray();

        //    var message =
        //        Encoding.UTF8.GetString(body);

        //    Console.WriteLine(
        //        $"Mensagem recebida: {message}");

        //    await channel.BasicAckAsync(
        //        deliveryTag: eventArgs.DeliveryTag,
        //        multiple: false);
        //};

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();

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

                using var scope =
                    _scopeFactory.CreateScope();

                var repository =
                    scope.ServiceProvider
                        .GetRequiredService<IProcessedEventRepository>();

                var jaProcessado =
                    await repository.ExisteAsync(
                        evento.CompraId);

                if (jaProcessado)
                {
                    Console.WriteLine(
                        $"Evento já processado: {evento.CompraId}");

                    await channel.BasicAckAsync(
                        eventArgs.DeliveryTag,
                        false);

                    return;
                }

                Console.WriteLine(
                    $"Processando compra: {evento.CompraId}");

                // Processamento real entrará aqui

                var processedEvent =
                    new ProcessedEvent(
                        evento.CompraId,
                        "CompraCriadaEvent");

                await repository.CriarAsync(
                    processedEvent);

                var context =
                    scope.ServiceProvider
                        .GetRequiredService<AppDbContext>();

                await context.SaveChangesAsync();

                await channel.BasicAckAsync(
                    eventArgs.DeliveryTag,
                    false);

                Console.WriteLine(
                    $"Evento processado: {evento.CompraId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Erro: {ex.Message}");

                await channel.BasicNackAsync(
                    eventArgs.DeliveryTag,
                    false,
                    true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: "financehub.compra.criada",
            autoAck: false,
            consumer: consumer);

        await Task.Delay(
            Timeout.Infinite,
            cancellationToken);
    }
}