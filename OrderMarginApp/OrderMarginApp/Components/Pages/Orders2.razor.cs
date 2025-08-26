using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace OrderMarginApp.Components.Pages;

public partial class Orders2 : ComponentBase
{
    private List<PriceCalculatorDto>? _prices;
    private const string _regexPattern = @"\s|zł";

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        _prices = ParseCsv(content);
    }

    private static List<PriceCalculatorDto> ParseCsv(string csv)
    {
        var result = new List<PriceCalculatorDto>();
        var lines = csv.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines.Skip(2)) // skip header
        {
            var cols = line.Split(',');

            if (cols.Length >= 5)
            {
                if (string.IsNullOrEmpty(cols[0]) && string.IsNullOrEmpty(cols[22]) && string.IsNullOrEmpty(cols[23]))
                {
                    break;
                }

                var price = new PriceCalculatorDto()
                {
                    Sku = cols[1].Trim(),
                    EstimatedShippingCostZl = decimal.TryParse(
                        Regex.Replace(cols[22], _regexPattern, ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var estimatedShippingCostZl)
                        ? estimatedShippingCostZl
                        : 0,
                    NetDeliveryCostZl = decimal.TryParse(
                        Regex.Replace(cols[23], _regexPattern, ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var netDeliveryCost)
                        ? netDeliveryCost
                        : 0,
                };
                result.Add(price);
            }
        }
        return result;
    }
}