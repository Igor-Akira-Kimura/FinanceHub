using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Carteira.Worker.Messaging
{
    public interface IRabbitMqConsumer
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
