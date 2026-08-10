using FinanceHub.Api.Requests;
using FluentValidation;

namespace FinanceHub.Api.Validators
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
