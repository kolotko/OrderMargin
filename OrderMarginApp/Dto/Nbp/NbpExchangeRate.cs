using System.Text.Json.Serialization;
using Domain;

namespace Dto.Nbp;

public class NbpExchangeRate
{
    public string? Table { get; set; }
    public string? Currency { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CurrencyCode Code { get; set; }
    public List<NbpRate>? Rates { get; set; }
}