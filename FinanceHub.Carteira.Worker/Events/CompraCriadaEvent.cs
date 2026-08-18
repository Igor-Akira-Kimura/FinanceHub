using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Carteira.Worker.Events
{
    public class CompraCriadaEvent
    {
        public Guid CompraId { get; set; }

        public Guid CarteiraId { get; set; }

        public Guid AtivoId { get; set; }

        public decimal Quantidade { get; set; }

        public decimal Preco { get; set; }
    }
}
