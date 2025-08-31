using Dto;
using FluentValidation;

namespace Validator;

public class OrderFileValidator : AbstractValidator<OrderFileDto>
{
    public OrderFileValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId nie może być pusty.");

        RuleFor(x => x.Date)
            .GreaterThan(new DateTime(2022, 12, 31))
            .WithMessage("Data musi być po roku 2022.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("ProductName nie może być puste.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId nie może być puste.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency nie może być puste.");

        RuleFor(x => x.ShippingMethod)
            .NotEmpty().WithMessage("ShippingMethod nie może być puste.");

        RuleFor(x => x.OrderStatus)
            .NotEmpty().WithMessage("OrderStatus nie może być puste.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity musi być większe od 0.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price musi być większe od 0.");

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("ShippingCost nie może być ujemny");
    }
}