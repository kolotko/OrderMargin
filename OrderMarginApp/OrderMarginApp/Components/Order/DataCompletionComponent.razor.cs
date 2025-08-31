using Abstraction.Services;
using Microsoft.AspNetCore.Components;

namespace OrderMarginApp.Components.Order;

public partial class DataCompletionComponent : ComponentBase
{
    [Inject]
    private INbpService? _nbpService { get; set; }
    // pobieranie walut
    // await _nbpService!.DownloadRatesFromTimeRange(DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)));
    // Console.WriteLine(_nbpService.GetRateForDay("EUR", DateOnly.FromDateTime(DateTime.Now.AddDays(-1))));
}