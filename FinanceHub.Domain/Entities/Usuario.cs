using FinanceHub.Domain.Exceptions;

namespace FinanceHub.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }

        public string Nome { get; private set; } = null!;

        public string Email { get; private set; } = null!;

        public string SenhaHash { get; private set; }

        public bool Ativo { get; private set; }

        public DateTime DataCriacao { get; private set; }

        public DateTime? DataAtualizacao { get; private set; }

        public ICollection<Carteira> Carteiras { get; private set; } = [];

        public Usuario(string nome, string email, string senhaHash)
        {
            DefinirDados(nome, email);

            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha é obrigatória.", nameof(senhaHash));

            Id = Guid.NewGuid();
            SenhaHash = senhaHash;
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
        }

        public void Atualizar(string nome, string email)
        {
            DefinirDados(nome, email);

            DataAtualizacao = DateTime.UtcNow;
        }

        private void DefinirDados(string nome, string email)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome é obrigatório.", nameof(nome));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O e-mail é obrigatório.", nameof(email));

            Nome = nome;
            Email = email;
        }

        public void Desativar()
        {
            if (!Ativo)
            {
                throw new UsuarioJaDesativadoException(Id);
            }

            Ativo = false;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}
