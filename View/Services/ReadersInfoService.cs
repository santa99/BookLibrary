using Contracts.Models;

namespace View.Services;

public class ReadersInfoService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReadersInfoService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ReadersInfo?> GetReadersInfo(int readersCardId)
    {
        using var client = CreateClient();

        var result = await client
            .GetFromJsonAsync<ReadersInfo>($"api/readers/get/{readersCardId}");

        return result;
    }

    /// <summary>
    /// Retrieves all readers.
    /// </summary>
    /// <returns>Readers info list.</returns>
    public async Task<List<ReadersInfo>> GetAllReaders()
    {
        using var client = CreateClient();

        var results = await client.GetFromJsonAsync<List<ReadersInfo>>("api/readers/select");
        return results ?? new List<ReadersInfo>();
    }

    private HttpClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://localhost:7227");
        return httpClient;
    }
}