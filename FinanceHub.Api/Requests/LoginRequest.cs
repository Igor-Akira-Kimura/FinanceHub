namespace FinanceHub.Api.Application.Requests.Auth
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;

        public string Senha { get; set; } = null!;
    }
}