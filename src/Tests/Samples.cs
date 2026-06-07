// ReSharper disable UnusedVariable

[TestFixture]
public class Samples
{
    [Test]
    public void PandocPath()
    {
        #region PandocPath

        var engine = new PandocEngine(@"D:\Tools\pandoc.exe");

        #endregion
    }

    [Test]
    public async Task Files()
    {
        #region files

        await PandocInstance.Convert<CommonMarkIn, HtmlOut>("sample.md", "output.html");

        #endregion

        await VerifyFile("output.html");
    }

    [Test]
    public async Task Streams()
    {
        {
            #region streams

            await using var inStream = File.OpenRead("sample.md");
            await using var outStream = File.OpenWrite("output.html");
            await PandocInstance.Convert<CommonMarkIn, HtmlOut>(inStream, outStream);

            #endregion
        }

        await VerifyFile("output.html");
    }

    [Test]
    public async Task Text()
    {
        #region text

        var html = await PandocInstance.ConvertToText<CommonMarkIn, HtmlOut>("*text*");

        #endregion

        await Verify(html.Value, "html");
    }

    [Test]
    [Explicit]
    public async Task HttpClientFactory()
    {
        #region http-client-factory

        var services = new ServiceCollection();
        services.AddHttpClient();
        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        // Adapt the IHttpClientFactory to the Func<HttpClient> the engine expects
        var engine = new PandocEngine(httpClientFactory: () => factory.CreateClient());

        var result = await engine.ConvertToText<CommonMarkIn, HtmlOut>(
            "https://raw.githubusercontent.com/Papyrine/PandocNet/main/readme.md");

        #endregion

        Assert.That(result.Value, Is.Not.Empty);
    }

    [Test]
    [Explicit]
    public async Task ReplicantCaching()
    {
        #region replicant-caching

        // Wire Replicant disk caching into the http client pipeline
        var services = new ServiceCollection();
        services.AddReplicantCache("cacheDirectory");
        services.AddHttpClient(string.Empty)
            .AddReplicantCaching();
        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        // Downloads of url inputs are now cached to disk
        var engine = new PandocEngine(httpClientFactory: () => factory.CreateClient());

        var result = await engine.ConvertToText<CommonMarkIn, HtmlOut>(
            "https://raw.githubusercontent.com/Papyrine/PandocNet/main/readme.md");

        #endregion

        Assert.That(result.Value, Is.Not.Empty);
    }

    [Test]
    [Explicit]
    public async Task CustomOptions()
    {
        #region custom-options

        var html = await PandocInstance.ConvertToText(
            """

            # Heading1

            text

            ## Heading2

            text

            """,
            new CommonMarkIn
            {
                ShiftHeadingLevelBy = 2
            },
            new HtmlOut
            {
                NumberOffsets = [3]
            });

        #endregion

        await Verify(html.Value, "html");
    }
}