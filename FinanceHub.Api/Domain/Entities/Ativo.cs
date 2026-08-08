using FinanceHub.Api.Domain.Enums;
using FinanceHub.Api.Domain.Exceptions;

namespace FinanceHub.Api.Domain.Entities
{
    public class Ativo
    {
        public Guid Id { get; private set; }

        public string Nome { get; private set; } = null!;

        public string Ticker { get; private set; } = null!;

        public TipoAtivo Tipo { get; private set; }

        public Guid BolsaId { get; private set; }

        public Bolsa Bolsa { get; private set; } = null!;

        public bool EstaAtivo { get; private set; }

        public DateTime DataCriacao { get; private set; }

        public DateTime? DataAtualizacao { get; private set; }

        public ICollection<Posicao> Posicoes { get; private set; } = [];

        private Ativo()
        {
        }

        public Ativo(string nome, string ticker, TipoAtivo tipo, Guid bolsaId)
        {
            DefinirDados(nome, ticker);

            Tipo = tipo;
            BolsaId = bolsaId;

            EstaAtivo = true;
            DataCriacao = DateTime.UtcNow;
        }

        public void Atualizar(string nome, string ticker, TipoAtivo tipo)
        {
            DefinirDados(nome, ticker);

            Tipo = tipo;

            DataAtualizacao = DateTime.UtcNow;
        }

        public void Desativar()
        {
            if (!EstaAtivo)
            {
                throw new AtivoJaDesativadoException(Id);
            }

            EstaAtivo = false;
            DataAtualizacao = DateTime.UtcNow;
        }

        private void DefinirDados(string nome, string ticker)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do ativo não pode ser vazio.", nameof(nome));
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("O ticker do ativo não pode ser vazio.", nameof(ticker));

            Nome = nome;
            Ticker = ticker;
        }
    }
}
