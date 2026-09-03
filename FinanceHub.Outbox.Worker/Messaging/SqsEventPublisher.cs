using Amazon.SQS;
using Amazon.SQS.Model;

namespace FinanceHub.Outbox.Worker.Messaging;

public class SqsEventPublisher : IEventPublisher
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _configuration;

    public SqsEventPublisher(
        IAmazonSQS sqs,
        IConfiguration configuration)
    {
        _sqs = sqs;
        _configuration = configuration;
    }

    public async Task PublishAsync(
        string type,
        string payload,
        CancellationToken cancellationToken)
    {
        var queueUrl =
            _configuration["SQS:QueueUrl"]
            ?? throw new InvalidOperationException(
                "SQS:QueueUrl não configurado.");

        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = payload
        };

        await _sqs.SendMessageAsync(
            request,
            cancellationToken);
    }
}