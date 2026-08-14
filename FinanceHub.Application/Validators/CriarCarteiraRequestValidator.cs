using FinanceHub.Application.Requests;
using FluentValidation;

namespace FinanceHub.Application.Validators
{
    public class CriarCarteiraRequestValidator
    : AbstractValidator<CriarCarteiraRequest>
    {
        public CriarCarteiraRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
