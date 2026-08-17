using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Application.Common.Events
{
    public record CompraCriadaEvent(
    Guid CompraId,
    Guid CarteiraId,
    Guid AtivoId,
    decimal Quantidade,
    decimal Preco);
}
