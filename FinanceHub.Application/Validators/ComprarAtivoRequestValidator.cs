using FluentValidation;

namespace FinanceHub.Application.Requests.Carteiras;

public class ComprarAtivoRequestValidator : AbstractValidator<ComprarAtivoRequest>
{
    public ComprarAtivoRequestValidator()
    {
        RuleFor(x => x.CarteiraId)
            .NotEmpty();

        RuleFor(x => x.AtivoId)
            .NotEmpty();

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");
    }
}