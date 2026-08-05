namespace FinanceHub.Api.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }

        public string Nome { get; private set; }

        public string Email { get; private set; }

        public string SenhaHash { get; private set; }

        public bool Ativo { get; private set; }

        public DateTime DataCriacao { get; private set; }

        public DateTime? DataAtualizacao { get; private set; }

        public Usuario(string nome, string email, string senhaHash)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório.", nameof(nome));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório.", nameof(email));

            if (string.IsNullOrWhiteSpace(senhaHash))
                throw new ArgumentException("Senha é obrigatória.", nameof(senhaHash));

            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
        }
    }
}
