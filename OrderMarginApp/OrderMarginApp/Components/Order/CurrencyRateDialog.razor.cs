using Domain;
using Microsoft.AspNetCore.Components;

namespace OrderMarginApp.Components.Order;

public partial class CurrencyRateDialog : ComponentBase
{
    [Parameter] public List<RateOfTheDay>? RatesForCurrency { get; set; }
}