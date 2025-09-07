using Abstraction.Services;
using Dto;
using Mapper;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace OrderMarginApp.Components.Order;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial class DataCompletionComponent : ComponentBase
{
    [Inject]
    private INbpService? _nbpService { get; set; }
    //
    [Parameter]
    public List<OrderFileDto>? OrdersDto { get; set; }

    [Parameter]
    public List<PriceCalculatorDto>? PricesDto { get; set; }

    private List<Domain.Order>? Orders;
    private RadzenDataGrid<Domain.Order> _grid;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (OrdersDto is null || PricesDto is null)
        {
            return;
        }

        Orders = OrdersDto.MapToOrder(PricesDto);
        var startDate = DateOnly.FromDateTime(Orders.Min(x => x.Date));
        var endDate = DateOnly.FromDateTime(Orders.Max(x => x.Date));
        await _nbpService!.DownloadRatesFromTimeRange(startDate, endDate);
        FillMissingData();
    }

    private void FillMissingData()
    {
        foreach (var order in Orders!)
        {
            order.TotalCost = order.Quantity * order.Price + order.ShippingCost;
            order.CurrencyExchangeRate = _nbpService!.GetRateForDay(order.Currency!, DateOnly.FromDateTime(order.Date));
            order.TotalCostPln = order.TotalCost * order.CurrencyExchangeRate;
        }
    }
    // order.TotalCost = order.Quantity * order.Price + order.ShippingCost;

    // https://blazor.radzen.com/master-detail-hierarchy-demand?theme=material3
    // pobieranie walut
    // await _nbpService!.DownloadRatesFromTimeRange(DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)));
    // Console.WriteLine(_nbpService.GetRateForDay("EUR", DateOnly.FromDateTime(DateTime.Now.AddDays(-1))));
}