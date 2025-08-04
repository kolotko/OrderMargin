using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using Abstraction.Services;
using Domain.Nbp;
using Dto.Nbp;

namespace Implementation.Services;

public class NbpService(HttpClient httpClient) : INbpService
{
    private HttpClient HttpClient { get; set; } = httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task GetData(DateOnly minDate, DateOnly maxTaxDate)
    {
        var fromDate = minDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var toDate = maxTaxDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        foreach (var currencyCode in NbpSettings.CurrencyCodes)
        {
            //zapisać klucz z enuma + kursy
            await DownloadData(currencyCode, fromDate, toDate);
        }
    }

    private async Task<NbpExchangeRate> DownloadData(string currencyCode, string fromDate, string toDate)
    {
        try
        {
            var response = await HttpClient.GetStringAsync(NbpSettings.ApiUrl + $"/{currencyCode}/{fromDate}/{toDate}");
            var exchangeData = JsonSerializer.Deserialize<NbpExchangeRate>(response, _options);

            return exchangeData!;
        }
        catch (Exception ex)
        {
            // TODO
            Console.WriteLine("Błąd zapytania HTTP: " + ex.Message);
        }
        // TODO
        throw new NotImplementedException();
    }
}