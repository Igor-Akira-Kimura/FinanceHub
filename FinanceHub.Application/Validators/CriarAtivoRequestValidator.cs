using FinanceHub.Application.Requests;
using FluentValidation;

namespace FinanceHub.Application.Validators
{
    public class CriarAtivoRequestValidator : AbstractValidator<CriarAtivoRequest>
    {
        public CriarAtivoRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.Ticker)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(10);

            RuleFor(x => x.Tipo)
                .IsInEnum();

            RuleFor(x => x.BolsaId)
                .NotEmpty();
        }
    }
}
