using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen.Blazor;
using Validator;

namespace OrderMarginApp.Components.Order;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial class PriceCalculatorComponent : ComponentBase
{
    [Parameter]
    public EventCallback<List<PriceCalculatorDto>> PriceCalculatorReady { get; set; }
    [Parameter]
    public List<PriceCalculatorDto>? Prices { get; set; }
    private PriceCalculatorDtoValidator? _validator;
    private const string _regexPattern = @"[^\d.]";
    private const string _regexPatternPercentage = @"\s|%";
    private RadzenDataGrid<PriceCalculatorDto> _grid;

    protected override async Task OnInitializedAsync()
    {
        _validator = new PriceCalculatorDtoValidator();
        await base.OnInitializedAsync();
    }

    private async Task OnUpdateRow(PriceCalculatorDto item)
    {
        item.ValidatorResult = ValidateRecord(item);
        await _grid.UpdateRow(item);
        await DataReady();
    }

    private async Task DeleteRow(PriceCalculatorDto item)
    {
        Prices!.Remove(item);
        await _grid.Reload();
        await DataReady();
    }

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        Prices = ParseCsv(content);
        await DataReady();
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

                var x = cols[26];
                var d = cols[27];
                var z = cols[28];
                var priceCalculatorDto = new PriceCalculatorDto()
                {
                    ProductName = cols[0].Trim(),
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
                    TotalCosts = decimal.TryParse(
                        Regex.Replace(cols[26], _regexPattern, ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var totalCosts)
                        ? totalCosts
                        : 0,
                    AmountMargin = decimal.TryParse(
                        Regex.Replace(cols[27], _regexPattern, ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var amountMargin)
                        ? amountMargin
                        : 0,
                    IncomePercentage = decimal.TryParse(
                        Regex.Replace(cols[28], _regexPatternPercentage, ""),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var incomePercentage)
                        ? incomePercentage
                        : 0,
                    ValidatorResult = string.Empty
                };

                priceCalculatorDto.ValidatorResult = ValidateRecord(priceCalculatorDto);
                result.Add(priceCalculatorDto);
            }
        }
        return result;
    }

    private string ValidateRecord(PriceCalculatorDto item)
    {
        var validatorResult = _validator!.Validate(item);
        if (!validatorResult.IsValid)
        {
            foreach (var error in validatorResult.Errors)
            {
                return error.ErrorMessage;
            }
        }

        return string.Empty;
    }

    private async Task DataReady()
    {
        if (PriceCalculatorReady.HasDelegate)
            await PriceCalculatorReady.InvokeAsync(Prices);
    }
}