using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace View.Services;

public class LoginService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<UserClaim?> LogUserIn(LoginReqModel model)
    {
        var httpClient = CreateClient();

        var authContent = new FormUrlEncodedContent(
            new List<KeyValuePair<string, string>>
            {
                new("username", model.Username),
                new("password", model.Password)
            }
        );

        var httpResponseMessage = await httpClient.PostAsync($"account/login?returnUrl=/", authContent);
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            var enumerable = httpResponseMessage.Headers.GetValues("set-cookie").FirstOrDefault();

            var strings = enumerable.Split("=");
            var key = strings[0];
            var value = strings[1];

            
            
            var userClaims = await GetUser(httpClient);
            var firstOrDefault = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (firstOrDefault != null)
            {
                return new UserClaim(firstOrDefault.Type, firstOrDefault.Value);
            }
        }

        return null;
    }

    public async Task LogUserOut()
    {
        var httpClient = CreateClient();

        var result = await httpClient.GetAsync($"account/logout");
    }

    public async Task<List<UserClaim>?> GetUser(HttpClient? client = null)
    {
        var httpClient = client ??= CreateClient();
        
        try
        {
            return await httpClient.GetFromJsonAsync<List<UserClaim>>("account/user");
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<bool>IsSignedIn()
    {
        var claims = await GetUser();
        if (claims?.Count > 0)
        {
            var isUser = claims.Select(claim => claim.Type == "Name" && claim.Value == "m").FirstOrDefault();
            return true;
        }

        return false;
    }

    public bool ShowUserLoginDialog { get; private set; }

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
        var httpClient = _httpClientFactory.CreateClient();

        httpClient.BaseAddress = new Uri("https://localhost:7227");

        return httpClient;
    }
}

public record LoginReqModel(
    string Username,
    [DataType(DataType.Password)] string Password
);

public record UserClaim(string Type, string Value);