using FinanceHub.Api.Interfaces.Services;

namespace FinanceHub.Api.Services
{
    public class PasswordService : IPasswordService
    {
        public string Hash(string senha)
            => BCrypt.Net.BCrypt.HashPassword(senha);

        public bool Verify(string senha, string hash)
            => BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}
