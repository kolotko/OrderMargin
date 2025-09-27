using Domain;

namespace Abstraction.Services;

public interface INbpService
{
    Task DownloadRatesFromTimeRange(DateOnly minDate, DateOnly maxTaxDate);
    decimal GetRateForDay(string currency, DateOnly date);
    List<RateOfTheDay> GetDownloadedRatesForCurrency(string currencyCode);
}