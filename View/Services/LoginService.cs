using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Claims;
using Newtonsoft.Json;

namespace View.Services;

public class LoginService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LoginService> _logger;

    public Action<AccountResponse> OnLoginFailure { get; set; }

    public LoginService(IHttpClientFactory httpClientFactory, ILogger<LoginService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
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
            var cookieHeader = httpResponseMessage.Headers.GetValues("set-cookie").FirstOrDefault();
            if (cookieHeader != null)
            {
                var strings = cookieHeader.Split("=");
                if (strings.Length > 1)
                {
                    var key = strings[0];
                    var value = strings[1];
                    _logger.LogDebug("key: {Key} value: {Value}", key, value);
                }
            }

            var userClaims = await GetUser(httpClient);
            var firstOrDefault = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (firstOrDefault != null)
            {
                return new UserClaim(firstOrDefault.Type, firstOrDefault.Value);
            }
        }
        else if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();
            try
            {
                var deserializeObject = JsonConvert.DeserializeObject<AccountResponse>(readAsStringAsync);

                OnLoginFailure?.Invoke(deserializeObject);
            }
            catch (SerializationException ex)
            {
            }
        }

        return null;
    }

    public record AccountResponse(bool IsSuccess, string Message);

    public async Task LogUserOut()
    {
        var httpClient = CreateClient();

        var result = await httpClient.GetAsync($"account/logout");
    }

    public async Task<List<UserClaim>> GetUser(HttpClient? client = null)
    {
        var httpClient = client ?? CreateClient();

        List<UserClaim> claims;
        try
        {
            claims = await httpClient.GetFromJsonAsync<List<UserClaim>>("account/user") ?? new List<UserClaim>();
        }
        catch (Exception e)
        {
            return new List<UserClaim>();
        }

        return claims;
    }

    public async Task<bool> IsSignedIn()
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