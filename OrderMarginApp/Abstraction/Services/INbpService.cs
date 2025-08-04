namespace Abstraction.Services;

public interface INbpService
{
    Task GetData(DateOnly minDate, DateOnly maxTaxDate);
}