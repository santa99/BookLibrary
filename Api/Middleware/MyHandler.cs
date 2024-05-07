namespace Api.Middleware;

public class MyHandler : DelegatingHandler
{
    async protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        
        return response;
    }
}