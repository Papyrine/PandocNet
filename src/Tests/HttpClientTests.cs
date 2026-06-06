[TestFixture]
public class HttpClientTests
{
    class FakePandocHttpClient : IPandocHttpClient
    {
        public string? RequestedUrl;

        public Task<Stream> GetStream(string url, Cancel cancel = default)
        {
            RequestedUrl = url;
            Stream stream = new MemoryStream("*from-url*"u8.ToArray());
            return Task.FromResult(stream);
        }
    }

    [Test]
    public async Task UrlInputUsesProvidedHttpClient()
    {
        var httpClient = new FakePandocHttpClient();
        var engine = new PandocEngine(httpClient: httpClient);

        var result = await engine.ConvertToText<CommonMarkIn, HtmlOut>("https://example.com/sample.md");

        Assert.That(httpClient.RequestedUrl, Is.EqualTo("https://example.com/sample.md"));
        Assert.That(result.Value, Does.Contain("<em>from-url</em>"));
    }
}
