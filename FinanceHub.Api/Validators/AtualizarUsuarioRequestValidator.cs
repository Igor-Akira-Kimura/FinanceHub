using FinanceHub.Api.Requests;
using FluentValidation;

namespace FinanceHub.Api.Validators
{
    public class AtualizarUsuarioRequestValidator : AbstractValidator<AtualizarUsuarioRequest>
    {
        public AtualizarUsuarioRequestValidator()
        {
            RuleFor(x => x.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O nome é obrigatório.")
                .MinimumLength(3)
                .WithMessage("O nome deve ter pelo menos 3 caracteres.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório.")
                .EmailAddress()
                .WithMessage("O e-mail deve ser um endereço válido.");
        }
    }
}
