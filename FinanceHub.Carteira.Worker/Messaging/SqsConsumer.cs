using Amazon.SQS;
using Amazon.SQS.Model;
using FinanceHub.Application.Common.Events;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using System.Text.Json;

namespace FinanceHub.Carteira.Worker.Messaging;

public class SqsConsumer : IMessageConsumer
{
    private readonly IAmazonSQS _sqs;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public SqsConsumer(
        IAmazonSQS sqs,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _sqs = sqs;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var queueUrl =
            _configuration["SQS:QueueUrl"]
            ?? throw new InvalidOperationException(
                "SQS:QueueUrl não configurado.");

        Console.WriteLine("SQS Consumer iniciado.");
        Console.WriteLine($"Queue: {queueUrl}");

        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(
                new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20,
                    VisibilityTimeout = 30
                },
                cancellationToken);

            foreach (var message in response.Messages)
            {
                await ProcessarMensagemAsync(
                    queueUrl,
                    message,
                    cancellationToken);
            }
        }
    }

    private async Task ProcessarMensagemAsync(
        string queueUrl,
        Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine(
                $"Mensagem recebida: {message.MessageId}");

            var evento =
                JsonSerializer.Deserialize<CompraCriadaEvent>(
                    message.Body);

            if (evento is null)
                throw new Exception(
                    "Não foi possível desserializar o evento.");

            if (evento.EventId == Guid.Empty)
                throw new Exception("EventId inválido.");

            if (evento.CompraId == Guid.Empty)
                throw new Exception("CompraId inválido.");

            if (evento.CarteiraId == Guid.Empty)
                throw new Exception("CarteiraId inválido.");

            if (evento.AtivoId == Guid.Empty)
                throw new Exception("AtivoId inválido.");

            if (evento.Quantidade <= 0)
                throw new Exception("Quantidade inválida.");

            if (evento.Preco <= 0)
                throw new Exception("Preço inválido.");

            using var scope =
                _scopeFactory.CreateScope();

            var repository =
                scope.ServiceProvider
                    .GetRequiredService<IProcessedEventRepository>();

            var context =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            var jaProcessado =
                await repository.ExisteAsync(evento.EventId);

            if (jaProcessado)
            {
                Console.WriteLine(
                    $"Evento já processado: {evento.EventId}");

                await _sqs.DeleteMessageAsync(
                    queueUrl,
                    message.ReceiptHandle,
                    cancellationToken);

                return;
            }

            await using var transaction =
                await context.Database
                    .BeginTransactionAsync(cancellationToken);

            try
            {
                Console.WriteLine(
                    $"Processando compra: {evento.CompraId}");

                var processedEvent =
                    new ProcessedEvent(
                        evento.EventId,
                        nameof(CompraCriadaEvent));

                await repository.CriarAsync(
                    processedEvent);

                await context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }

            // Só removemos do SQS depois do COMMIT.
            await _sqs.DeleteMessageAsync(
                queueUrl,
                message.ReceiptHandle,
                cancellationToken);

            Console.WriteLine(
                $"Evento processado com sucesso: {evento.EventId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Erro ao processar mensagem {message.MessageId}: {ex.Message}");

            // NÃO deletamos a mensagem.
            // O Visibility Timeout irá expirar
            // e o SQS poderá entregá-la novamente.
        }
    }
}