using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen.Blazor;
using Validator;

namespace OrderMarginApp.Components.Order;

public partial class PriceCalculatorComponent : ComponentBase
{
    [Parameter]
    public EventCallback<List<PriceCalculatorDto>> PriceCalculatorReady { get; set; }

    private List<PriceCalculatorDto>? _prices;
    private PriceCalculatorDtoValidator? _validator;
    private const string _regexPattern = @"\s|zł";
    private RadzenDataGrid<PriceCalculatorDto> _grid;

    protected override async Task OnInitializedAsync()
    {
        _validator = new PriceCalculatorDtoValidator();
        await base.OnInitializedAsync();
    }

    private void OnUpdateRow(PriceCalculatorDto item)
    {
        item.ValidatorResult = "";
        _grid.UpdateRow(item);
    }

    private void DeleteRow(PriceCalculatorDto item)
    {
        _prices!.Remove(item);
        _grid.Reload();
    }

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        _prices = ParseCsv(content);
    }

    private List<PriceCalculatorDto> ParseCsv(string csv)
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

                var priceCalculatorDto = new PriceCalculatorDto()
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
                    ValidatorResult = string.Empty
                };
                var validatorResult = _validator!.Validate(priceCalculatorDto);
                if (!validatorResult.IsValid)
                {
                    foreach (var error in validatorResult.Errors)
                    {
                        priceCalculatorDto.ValidatorResult = error.ErrorMessage;
                    }
                }
                result.Add(priceCalculatorDto);
            }
        }
        return result;
    }

    private async Task DataReady()
    {
        if (PriceCalculatorReady.HasDelegate)
            await PriceCalculatorReady.InvokeAsync(_prices);
    }
}