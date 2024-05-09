using Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace View.Services;

public class ReadersInfoService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigationManager;

    public ReadersInfoService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
    {
        _httpClientFactory = httpClientFactory;
        _navigationManager = navigationManager;
    }

    public async Task<ReadersInfo> GetReadersInfo(int readersCardId)
    {
        return await _httpClientFactory.CreateClient().GetFromJsonAsync<ReadersInfo>(_navigationManager.BaseUri + $"readers/{readersCardId}");
    }

    public async Task<List<ReadersInfo>> GetAllReaders()
    {
        return await _httpClientFactory.CreateClient().GetFromJsonAsync<List<ReadersInfo>>(_navigationManager.BaseUri + "readers");
    }
}