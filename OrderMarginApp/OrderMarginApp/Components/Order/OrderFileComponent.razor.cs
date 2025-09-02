using System.Globalization;
using System.Text;
using Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen.Blazor;
using Validator;

namespace OrderMarginApp.Components.Order;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public partial class OrderFileComponent : ComponentBase
{
    [Parameter]
    public EventCallback<List<OrderFileDto>> OrderFileReady { get; set; }

    [Parameter]
    public List<OrderFileDto>? Orders { get; set; }
    private OrderFileValidator? _validator;
    private RadzenDataGrid<OrderFileDto> _grid;

    protected override async Task OnInitializedAsync()
    {
        _validator = new OrderFileValidator();
        await base.OnInitializedAsync();
    }

    private async Task OnUpdateRow(OrderFileDto item)
    {
        item.ValidatorResult = ValidateRecord(item);
        await _grid.UpdateRow(item);
        await DataReady();
    }

    private async Task DeleteRow(OrderFileDto item)
    {
        Orders!.Remove(item);
        await _grid.Reload();
        await DataReady();
    }

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        Orders = ParseCsv(content);
        await DataReady();
    }

    private List<OrderFileDto> ParseCsv(string csv)
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
                            "M/d/yyyy H:mm",
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
                order.ValidatorResult = ValidateRecord(order);
                result.Add(order);
            }
        }

        return result;
    }

    private string ValidateRecord(OrderFileDto item)
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
        if (OrderFileReady.HasDelegate)
            await OrderFileReady.InvokeAsync(Orders);
    }
}