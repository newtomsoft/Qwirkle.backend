namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WebApis;

public abstract class WebApiBase
{
    protected readonly HttpClient _httpClient;
    protected const string ApiPrefix = "api";

    protected WebApiBase(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}