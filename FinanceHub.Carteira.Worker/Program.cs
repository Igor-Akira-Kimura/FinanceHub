using Amazon.SQS;
using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Carteira.Worker.Messaging;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Carteira.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var connectionString =
                builder.Configuration
                    .GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            //builder.Services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();

            builder.Services.AddSingleton<IMessageConsumer, SqsConsumer>();

            builder.Services.AddScoped<
                IProcessedEventRepository,
                ProcessedEventRepository>();

            builder.Services.AddDefaultAWSOptions(
            builder.Configuration.GetAWSOptions());

            builder.Services.AddAWSService<IAmazonSQS>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();

            host.Run();
        }
    }
}