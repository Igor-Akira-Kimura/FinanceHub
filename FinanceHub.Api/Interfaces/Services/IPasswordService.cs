namespace FinanceHub.Api.Interfaces.Services
{
    public interface IPasswordService
    {
        string Hash(string senha);

        bool Verify(string senha, string hash);
    }
}
