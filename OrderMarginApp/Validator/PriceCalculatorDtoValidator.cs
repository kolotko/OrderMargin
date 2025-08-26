using Dto;
using FluentValidation;

namespace Validator;

public class PriceCalculatorDtoValidator : AbstractValidator<PriceCalculatorDto>
{
    public PriceCalculatorDtoValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU nie może być puste.")
            .Must(sku => !string.IsNullOrWhiteSpace(sku))
            .WithMessage("SKU musi zawierać znaki.");

        RuleFor(x => x.EstimatedShippingCostZl)
            .GreaterThan(0)
            .WithMessage("Szacowany koszt wysyłki musi być większy niż 0.");

        RuleFor(x => x.NetDeliveryCostZl)
            .GreaterThan(0)
            .WithMessage("Koszt dostawy netto musi być większy niż 0.");
    }
}