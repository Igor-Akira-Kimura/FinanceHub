using FinanceHub.Domain.Exceptions;

namespace FinanceHub.Domain.Entities
{
    public class Bolsa
    {
        public Guid Id { get; private set; }

        public string Nome { get; private set; } = null!;

        public string Pais { get; private set; } = null!;

        public string Moeda { get; private set; } = null!;

        public bool Ativa { get; private set; }

        public DateTime DataCriacao { get; private set; }

        public DateTime? DataAtualizacao { get; private set; }

        public ICollection<Ativo> Ativos { get; private set; } = [];

        private Bolsa()
        {
        }

        public Bolsa(string nome, string pais, string moeda)
        {
            DefinirDados(nome, pais, moeda);

            Id = Guid.NewGuid();
            Ativa = true;
            DataCriacao = DateTime.UtcNow;
        }

        public void Atualizar(string nome, string pais, string moeda)
        {
            DefinirDados(nome, pais, moeda);

            DataAtualizacao = DateTime.UtcNow;
        }

        public void Desativar()
        {
            if (!Ativa)
            {
                throw new BolsaJaDesativadaException(Id);
            }

            Ativa = false;
            DataAtualizacao = DateTime.UtcNow;
        }

        private void DefinirDados(string nome, string pais, string moeda)
        {
            if(string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da bolsa é obrigatório.", nameof(nome));
            if(string.IsNullOrWhiteSpace(pais))
                throw new ArgumentException("O país da bolsa é obrigatório.", nameof(pais));
            if(string.IsNullOrWhiteSpace(moeda))
                throw new ArgumentException("A moeda da bolsa é obrigatória.", nameof(moeda));

            Nome = nome;
            Pais = pais;
            Moeda = moeda;
        }
    }
}
