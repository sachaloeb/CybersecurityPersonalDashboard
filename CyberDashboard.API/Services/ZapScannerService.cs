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
                Url = alert.GetProperty("url").GetString(),
                Description = alert.TryGetProperty("description", out var desc) ? desc.GetString() : null,
                Solution = alert.TryGetProperty("solution", out var sol) ? sol.GetString() : null,
                Reference = alert.TryGetProperty("reference", out var refVal) ? refVal.GetString() : null,
                Tags = alert.TryGetProperty("tags", out var tags) && tags.ValueKind == JsonValueKind.Object
                    ? tags.EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetString() ?? "")
                    : new Dictionary<string, string>()
            });
        }

        return alerts;
    }
    
    public async Task<List<CveEntry>> GetCvesForKeywordAsync(string keyword)
    {
        var encodedKeyword = Uri.EscapeDataString(keyword);
        Console.WriteLine($"Encoded keyword: {encodedKeyword}");
        var response = await _httpClient.GetAsync(
            $"https://services.nvd.nist.gov/rest/json/cves/2.0?keywordSearch={encodedKeyword}&resultsPerPage=5");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"NVD API failed: {response.StatusCode}");
            return new List<CveEntry>(); // Return empty gracefully
        }

        var json = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(json);
        var result = new List<CveEntry>();

        foreach (var item in root.RootElement.GetProperty("vulnerabilities").EnumerateArray())
        {
            var cve = item.GetProperty("cve");
            result.Add(new CveEntry
            {
                Id = cve.GetProperty("id").GetString(),
                Description = cve.GetProperty("descriptions")[0].GetProperty("value").GetString(),
                Severity = cve.TryGetProperty("metrics", out var metrics) &&
                           metrics.TryGetProperty("cvssMetricV31", out var cvssArray) &&
                           cvssArray[0].TryGetProperty("cvssData", out var cvssData)
                    ? cvssData.GetProperty("baseSeverity").GetString()
                    : "Unknown",
                Published = cve.GetProperty("published").GetString()
            });
        }

        return result;
    }

	public async Task StartScanAsync(string url)
	{
    // 🧼 Start a new session to reset previous alerts
    await _httpClient.GetAsync("http://localhost:8080/JSON/core/action/newSession/?name=scanSession&overwrite=true");

    // 🕷️ Start spider scan
    await _httpClient.GetAsync($"http://localhost:8080/JSON/spider/action/scan/?url={url}");

    // ⏳ Wait until spidering is complete
    while (true)
    {
        var response = await _httpClient.GetStringAsync("http://localhost:8080/JSON/spider/view/status/");
        var status = JsonDocument.Parse(response).RootElement.GetProperty("status").GetString();
        if (status == "100") break;
        await Task.Delay(2000);
    }
	}

}