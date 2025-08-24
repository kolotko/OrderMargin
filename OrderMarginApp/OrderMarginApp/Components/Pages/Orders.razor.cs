using System.Globalization;
using System.Text;
using Abstraction.Services;
using Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace OrderMarginApp.Components.Pages;

public partial class Orders : ComponentBase
{
    [Inject]
    private INbpService? _nbpService { get; set; }

    private List<OrderFileDto>? _orders;

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        // pobieranie walut
        await _nbpService!.DownloadRatesFromTimeRange(DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), DateOnly.FromDateTime(DateTime.Now.AddDays(-1)));
        Console.WriteLine(_nbpService.GetRateForDay("EUR", DateOnly.FromDateTime(DateTime.Now.AddDays(-1))));

        var file = e.File;
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        _orders = ParseCsv(content);
    }

    private static List<OrderFileDto> ParseCsv(string csv)
    {
        var result = new List<OrderFileDto>();
        var lines = csv.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Skip(1)) // skip header
        {
            var cols = line.Split(',');

            if (cols.Length >= 5)
            {
                var order = new OrderFileDto
                {
                    OrderId = cols[0].Trim(),
                    Date =
                        DateTime.TryParseExact(
                            cols[1],
                            "M/d/yyyy HH:mm",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var date)
                            ? date
                            : DateTime.MinValue,
                    Source = cols[2].Trim(),
                    LastName = cols[3].Trim(),
                    Phone = cols[4].Trim(),
                    Email = cols[5].Trim(),
                    ProductName = cols[6].Trim(),
                    ProductId = cols[7].Trim(),
                    Sku = cols[8].Trim(),
                    Ean = cols[9].Trim(),
                    Quantity =
                        int.TryParse(cols[10], NumberStyles.Any, CultureInfo.InvariantCulture, out var quantity)
                            ? quantity
                            : 0,
                    Price =
                        decimal.TryParse(cols[11], NumberStyles.Any, CultureInfo.InvariantCulture, out var price)
                            ? price
                            : 0,
                    Currency = cols[12].Trim(),
                    ShippingCost =
                        decimal.TryParse(cols[13], NumberStyles.Any, CultureInfo.InvariantCulture, out var shippingCost)
                            ? shippingCost
                            : 0,
                    ShippingMethod = cols[14].Trim(),
                    TrackingNumber = cols[15].Trim(),
                    OrderStatus = cols[16].Trim(),
                };

                order.TotalCost = order.Quantity * order.Price + order.ShippingCost;
                result.Add(order);
            }
        }

        return result;
    }
}

// nbp 
// walidacja
// 2 plik
// posprzątać blazor