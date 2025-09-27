namespace Domain;

public class Order
{
    public string? OrderId { get; set; }
    public DateTime Date { get; set; }
    public string? Source { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProductName { get; set; }
    public string? ProductId { get; set; }
    public string? Sku { get; set; }
    public string? Ean { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Currency { get; set; }
    public decimal ShippingCost { get; set; }
    public string? ShippingMethod { get; set; }
    public string? TrackingNumber { get; set; }
    public string? OrderStatus { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalCostPln { get; set; }
    public decimal CurrencyExchangeRate { get; set; }
    public decimal Wyliczonawartosc { get; set; }
    public List<PriceCalculator>? PriceCalculator { get; set; }
}