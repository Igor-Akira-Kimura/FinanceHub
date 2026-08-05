using System.ComponentModel.DataAnnotations;

namespace FinanceHub.Api.Requests
{
    public class CriarUsuarioRequest
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Senha { get; set; } = string.Empty;
    }
}