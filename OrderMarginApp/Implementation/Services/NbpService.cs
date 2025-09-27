using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using Abstraction.Services;
using Domain;
using Domain.Nbp;
using Dto.Nbp;

namespace Implementation.Services;

public class NbpService(HttpClient httpClient) : INbpService
{
    private HttpClient HttpClient { get; set; } = httpClient;
    private Dictionary<string, List<RateOfTheDay>> _cache = new();

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task DownloadRatesFromTimeRange(DateOnly minDate, DateOnly maxTaxDate)
    {
        var overriddenMinDate = GetStartDate(minDate);
        _cache = new();
        foreach (var currencyCode in NbpSettings.CurrencyCodes)
        {
            var eurData = await DownloadData(currencyCode, overriddenMinDate, maxTaxDate);
            var eurRates = FillMissingDates(eurData, overriddenMinDate, maxTaxDate);
            _cache.Add(currencyCode, eurRates);
        }
    }

    public decimal GetRateForDay(string currency, DateOnly date)
    {
        if (currency == "PLN")
        {
            return 1;
        }

        var rates = _cache[currency];
        var record = rates.FirstOrDefault(x => x.Date == date);
        return record!.Rate;
    }

    public List<RateOfTheDay> GetDownloadedRatesForCurrency(string currencyCode)
    {
        return _cache[currencyCode];
    }

    private async Task<NbpExchangeRate> DownloadData(string currencyCode, DateOnly fromDate, DateOnly toDate)
    {
        try
        {
            var response = await HttpClient.GetStringAsync(NbpSettings.ApiUrl + $"/{currencyCode}/{fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}/{toDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");
            var exchangeData = JsonSerializer.Deserialize<NbpExchangeRate>(response, _options);

            return exchangeData!;
        }
        catch (Exception ex)
        {
            // TODO
            Console.WriteLine("Błąd zapytania HTTP: " + ex.Message);
            throw;
        }
    }

    private static List<RateOfTheDay> FillMissingDates(NbpExchangeRate data, DateOnly start, DateOnly end)
    {
        var result = new List<RateOfTheDay>();
        decimal lastRate = 0;

        try
        {
            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (data.Rates is null)
                    continue;

                var rateForDay = data.Rates.FirstOrDefault(r => r.EffectiveDate == date);

                if (rateForDay != null)
                {
                    lastRate = rateForDay.Mid;
                    result.Add(new RateOfTheDay
                    {
                        Date = date,
                        Rate = lastRate
                    });
                    continue;
                }

                if (lastRate != 0) // brak kursu (weekend/święto) weź ostatni kurs
                {
                    result.Add(new RateOfTheDay
                    {
                        Date = date,
                        Rate = lastRate
                    });
                }
            }
        }
        catch (Exception ex)
        {
            // TODO
            Console.WriteLine("Błąd zapytania HTTP: " + ex.Message);
            throw;
        }

        return result;
    }

    private static DateOnly GetStartDate(DateOnly minTaxDate)
    {
        switch (minTaxDate.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                return minTaxDate.AddDays(-1);
            case DayOfWeek.Sunday:
                return minTaxDate.AddDays(-2);
            default:
                return minTaxDate;
        }
    }
}