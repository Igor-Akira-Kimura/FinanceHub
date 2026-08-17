using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Outbox.Worker.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Outbox.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

            builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();

            host.Run();
        }
    }
}