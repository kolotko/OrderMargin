namespace Dto;

public class PriceCalculatorDto
{
    public string? ProductName { get; set; }
    public string? Sku { get; set; }
    public decimal EstimatedShippingCostZl { get; set; }
    public decimal NetDeliveryCostZl { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal AmountMargin { get; set; }
    public decimal IncomePercentage { get; set; }
    public string? ValidatorResult { get; set; }
}