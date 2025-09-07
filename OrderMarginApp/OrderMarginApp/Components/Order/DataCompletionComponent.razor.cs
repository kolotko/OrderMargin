using Dto;
using Mapper;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace OrderMarginApp.Components.Order;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial class DataCompletionComponent : ComponentBase
{
    // [Inject]
    // private INbpService? _nbpService { get; set; }
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
        if (OrdersDto is not null && PricesDto is not null)
        {
            Orders = OrdersDto.MapToOrder(PricesDto);
        }
    }
    // https://blazor.radzen.com/master-detail-hierarchy-demand?theme=material3
    // pobieranie walut
    // await _nbpService!.DownloadRatesFromTimeRange(DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)));
    // Console.WriteLine(_nbpService.GetRateForDay("EUR", DateOnly.FromDateTime(DateTime.Now.AddDays(-1))));
}