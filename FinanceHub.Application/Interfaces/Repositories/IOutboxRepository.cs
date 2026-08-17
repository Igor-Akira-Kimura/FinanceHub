using FinanceHub.Application.Common.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Application.Interfaces.Repositories
{
    public interface IOutboxRepository
    {
        Task CriarAsync(OutboxMessage message);
    }
}
