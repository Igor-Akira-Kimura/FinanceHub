using FinanceHub.Application.Requests;
using FluentValidation;

public class VenderAtivoRequestValidator : AbstractValidator<VenderAtivoRequest>
{
    public VenderAtivoRequestValidator()
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