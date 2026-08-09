using FluentValidation;

namespace FinanceHub.Api.Application.Requests.Carteiras;

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

        RuleFor(x => x.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.");
    }
}