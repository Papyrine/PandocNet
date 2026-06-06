namespace Pandoc;

class DefaultPandocHttpClient : IPandocHttpClient
{
    static HttpClient httpClient = new();

    public Task<Stream> GetStream(string url, Cancel cancel = default) =>
        httpClient.GetStreamAsync(url, cancel);
}
