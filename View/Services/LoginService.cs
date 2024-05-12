using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace View.Services;

public class LoginService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigationManager;
    private LoginResModel? _login;

    public LoginService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
    {
        _httpClientFactory = httpClientFactory;
        _navigationManager = navigationManager;
        _login = null;
    }

    public async Task<LoginResModel?> LogUserIn(LoginReqModel model)
    {
        if (IsUserLogged())
        {
            return _login;
        }
        
        var httpClient = CreateClient();
        
        var result = await httpClient.PostAsync(_navigationManager.BaseUri + $"account/login", JsonContent.Create(model));

        _login =  new LoginResModel("Janko");

        return _login;
    }

    public async Task LogUserOut()
    {
        if (!IsUserLogged())
        {
            return;
        }

        var httpClient = CreateClient();

        var result = await httpClient.GetAsync(_navigationManager.BaseUri + $"account/logout");
    }

    public bool ShowUserLoginDialog { get; private set; }
    public bool IsUserLogged()
    {
        return _login != null;
    }

    public void ShowLoginDialog()
    {
        ShowUserLoginDialog = true;
    }
    
    public void CloseLoginDialog()
    {
        ShowUserLoginDialog = false;
    }
    
    private HttpClient CreateClient()
    {
        var uri = new Uri("https://localhost:7227");
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler()
        {
            
        };
        // handler.CookieContainer = cookieContainer;
        var client = new HttpClient(handler);
        client.BaseAddress = uri;
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        return client;
    }
}

public record LoginResModel(
    string Username
);

public record LoginReqModel(
    string Username,
    [DataType(DataType.Password)] 
    string Password
);