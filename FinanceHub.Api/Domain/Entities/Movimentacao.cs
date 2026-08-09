using FinanceHub.Api.Domain.Enums;

namespace FinanceHub.Api.Domain.Entities
{
    public class Movimentacao
    {
        public Guid Id { get; private set; }

        public Guid PosicaoId { get; private set; }

        public TipoMovimentacao Tipo { get; private set; }

        public decimal Quantidade { get; private set; }

        public decimal Preco { get; private set; }

        public DateTime DataMovimentacao { get; private set; }

        public Posicao Posicao { get; private set; } = null!;

        private Movimentacao(Guid posicaoId, TipoMovimentacao tipo, decimal quantidade, decimal preco)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

            if (preco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.", nameof(preco));

            Id = Guid.NewGuid();
            PosicaoId = posicaoId;
            Tipo = tipo;
            Quantidade = quantidade;
            Preco = preco;
            DataMovimentacao = DateTime.UtcNow;
        }

        public static Movimentacao CriarCompra(Guid posicaoId, decimal quantidade, decimal preco)
        {
            return new Movimentacao(posicaoId, TipoMovimentacao.Compra, quantidade, preco);
        }

        public static Movimentacao CriarVenda(Guid posicaoId, decimal quantidade, decimal preco)
        {
            return new Movimentacao(posicaoId, TipoMovimentacao.Venda, quantidade, preco);
        }
    }
}
