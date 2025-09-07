namespace Domain;

public class PriceCalculator
{
    public string? ProductName { get; set; }
    public decimal EstimatedShippingCostZl { get; set; }
    public decimal NetDeliveryCostZl { get; set; }
    public string? ValidatorResult { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal AmountMargin { get; set; }
    public decimal IncomePercentage { get; set; }
}