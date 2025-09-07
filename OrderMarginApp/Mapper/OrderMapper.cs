using Domain;
using Dto;

namespace Mapper;

public static class OrderMapper
{
    public static List<Order> MapToOrder(this List<OrderFileDto> orderFileDtos, List<PriceCalculatorDto> priceCalculatorDtos)
    {
        var result = new List<Order>();
        foreach (var orderFileDto in orderFileDtos)
        {
            var tmpOrder = new Order
            {
                OrderId = orderFileDto.OrderId,
                Date = orderFileDto.Date,
                Source = orderFileDto.Source,
                LastName = orderFileDto.LastName,
                Phone = orderFileDto.Phone,
                Email = orderFileDto.Email,
                ProductName = orderFileDto.ProductName,
                ProductId = orderFileDto.ProductId,
                Sku = orderFileDto.Sku,
                Ean = orderFileDto.Ean,
                Quantity = orderFileDto.Quantity,
                Price = orderFileDto.Price,
                Currency = orderFileDto.Currency,
                ShippingCost = orderFileDto.ShippingCost,
                ShippingMethod = orderFileDto.ShippingMethod,
                TrackingNumber = orderFileDto.TrackingNumber,
                OrderStatus = orderFileDto.OrderStatus,
            };

            var fetchPriceCalculatorDtos = priceCalculatorDtos.Where(x => x.Sku == tmpOrder.Sku).ToList();
            tmpOrder.PriceCalculator = fetchPriceCalculatorDtos.MapToPriceCalculator();
            result.Add(tmpOrder);
        }

        return result;
    }

    private static List<PriceCalculator> MapToPriceCalculator(this List<PriceCalculatorDto> priceCalculatorDtos)
    {
        var result = new List<PriceCalculator>();
        foreach (var priceCalculatorDto in priceCalculatorDtos)
        {
            result.Add(new PriceCalculator
            {
                ProductName = priceCalculatorDto.ProductName,
                EstimatedShippingCostZl = priceCalculatorDto.EstimatedShippingCostZl,
                NetDeliveryCostZl = priceCalculatorDto.NetDeliveryCostZl,
                TotalCosts = priceCalculatorDto.TotalCosts,
                AmountMargin = priceCalculatorDto.AmountMargin,
                IncomePercentage = priceCalculatorDto.IncomePercentage,
            });
        }

        return result;
    }
}