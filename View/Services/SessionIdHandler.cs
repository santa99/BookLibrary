namespace View.Services;

public class SessionIdHandler : DelegatingHandler
{
    public static string Token = "auth";

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string authToken = null;
        if (request.Headers == null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var cookie = request.Headers.GetValues("set-cookie").FirstOrDefault();
        var valuePairs = cookie?.Split(";");
        Dictionary<string, string> keys = new Dictionary<string, string>();
        if (valuePairs != null)
        {
            foreach (var pair in valuePairs)
            {
                var strings = pair.Split("=");
                var key = strings[0];
                var value = strings[1];
                keys.Add(key, value);
            }

            keys.TryGetValue(Token, out authToken);
        }


        // Store the session ID in the request property bag.
        if (authToken != null)
        {
            request.Properties[Token] = authToken;
        }

        // Continue processing the HTTP request.
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (authToken != null)
        {
            response.Headers.Add(Token, authToken);
        }

        return response;
    }
}