using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Outbox.Worker.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync(
            string type,
            string payload,
            CancellationToken cancellationToken);
    }
}
