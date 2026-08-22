using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceHub.Domain.Entities
{
    public class Compra
    {
        public Guid Id { get; private set; }

        public Guid CarteiraId { get; private set; }

        public Guid AtivoId { get; private set; }

        public decimal Quantidade { get; private set; }

        public decimal Preco { get; private set; }

        public string IdempotencyKey { get; private set; } = null!;

        private Compra()
        {
        }

        public Compra(
            Guid carteiraId,
            Guid ativoId,
            decimal quantidade,
            decimal preco,
            string idempotencyKey)
        {
            if (carteiraId == Guid.Empty)
                throw new ArgumentException(
                    "Carteira inválida.",
                    nameof(carteiraId));

            if (ativoId == Guid.Empty)
                throw new ArgumentException(
                    "Ativo inválido.",
                    nameof(ativoId));

            if (quantidade <= 0)
                throw new ArgumentException(
                    "Quantidade deve ser maior que zero.",
                    nameof(quantidade));

            if (preco <= 0)
                throw new ArgumentException(
                    "Preço deve ser maior que zero.",
                    nameof(preco));

            Id = Guid.NewGuid();
            CarteiraId = carteiraId;
            AtivoId = ativoId;
            Quantidade = quantidade;
            Preco = preco;
            IdempotencyKey = idempotencyKey;
        }
    }
}
