namespace Dto;

public class PriceCalculatorDto
{
    public string? Sku { get; set; }
    public decimal EstimatedShippingCostZl { get; set; }
    public decimal NetDeliveryCostZl { get; set; }
    public string? ValidatorResult { get; set; }
}