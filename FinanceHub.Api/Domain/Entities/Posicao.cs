using FinanceHub.Api.Domain.Exceptions;

namespace FinanceHub.Api.Domain.Entities
{
    public class Posicao
    {
        public Guid Id { get; private set; }

        public Guid CarteiraId { get; private set; }

        public Guid AtivoId { get; private set; }

        public decimal Quantidade { get; private set; }

        public decimal PrecoMedio { get; private set; }

        public DateTime DataCriacao { get; private set; }

        public DateTime? DataAtualizacao { get; private set; }

        public Carteira Carteira { get; private set; } = null!;

        public Ativo Ativo { get; private set; } = null!;

        public Posicao(
            Guid carteiraId,
            Guid ativoId,
            decimal quantidade,
            decimal precoMedio)
        {
            DefinirDados(quantidade, precoMedio);

            Id = Guid.NewGuid();
            CarteiraId = carteiraId;
            AtivoId = ativoId;
            DataCriacao = DateTime.UtcNow;
            DataAtualizacao = null;
        }

        private static void ValidarQuantidade(decimal quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));
        }

        private static void ValidarPreco(decimal preco)
        {
            if (preco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.", nameof(preco));
        }

        private static void ValidarDados(decimal quantidade, decimal precoMedio)
        {
            ValidarQuantidade(quantidade);
            ValidarPreco(precoMedio);
        }

        private void DefinirDados(decimal quantidade, decimal precoMedio)
        {
            ValidarDados(quantidade, precoMedio);

            Quantidade = quantidade;
            PrecoMedio = precoMedio;
        }

        public void Comprar(decimal quantidade, decimal preco)
        {
            ValidarDados(quantidade, preco);

            var novaQuantidade = Quantidade + quantidade;

            var valorAtual = Quantidade * PrecoMedio;

            var valorCompra = quantidade * preco;

            PrecoMedio = (valorAtual + valorCompra) / novaQuantidade;

            Quantidade = novaQuantidade;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Vender(decimal quantidade)
        {
            ValidarQuantidade(quantidade);

            if (quantidade > Quantidade)
            {
                throw new QuantidadeInsuficienteException(Id, Quantidade, quantidade);
            }

            Quantidade -= quantidade;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}
