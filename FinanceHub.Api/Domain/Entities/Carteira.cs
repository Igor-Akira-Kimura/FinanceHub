using FinanceHub.Api.Domain.Exceptions;

namespace FinanceHub.Api.Domain.Entities
{
    public class Carteira
    {
        public Guid Id { get; private set; }

        public string Nome { get; private set; } = null!;

        public Guid UsuarioId { get; private set; }

        public Usuario Usuario { get; private set; } = null!;

        public bool Ativa { get; private set; }

        public DateTime DataCriacao { get; private set; }

        public DateTime? DataAtualizacao { get; private set; }

        public ICollection<Posicao> Posicoes { get; private set; } = [];

        private Carteira() { }

        public Carteira(
            string nome,
            Guid usuarioId)
        {
            DefinirDados(nome);

            Id = Guid.NewGuid();

            UsuarioId = usuarioId;

            Ativa = true;

            DataCriacao = DateTime.UtcNow;
            DataAtualizacao = null;
        }

        public void Atualizar(string nome)
        {
            DefinirDados(nome);
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Desativar()
        {
            if (!Ativa)
                throw new CarteiraJaDesativadaException(Id);

            Ativa = false;
            DataAtualizacao = DateTime.UtcNow;
        }

        private void DefinirDados(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da carteira é obrigatório.", nameof(nome));

            Nome = nome.Trim();
        }
    }
}
