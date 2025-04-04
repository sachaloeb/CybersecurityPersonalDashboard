using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ZapScannerService
{
    private readonly HttpClient _httpClient;

    public ZapScannerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ZapAlert>> GetAlertsAsync()
    {
        var response = await _httpClient.GetAsync("http://localhost:8080/JSON/core/view/alerts/");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var root = JsonDocument.Parse(json);
        var alerts = new List<ZapAlert>();

        foreach (var alert in root.RootElement.GetProperty("alerts").EnumerateArray())
        {
            alerts.Add(new ZapAlert
            {
                Alert = alert.GetProperty("alert").GetString(),
                Risk = alert.GetProperty("risk").GetString(),
                Url = alert.GetProperty("url").GetString()
            });
        }

        return alerts;
    }
}