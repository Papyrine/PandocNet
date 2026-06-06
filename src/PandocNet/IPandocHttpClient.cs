namespace Pandoc;

/// <summary>
/// Abstraction over the HTTP access used to download <see cref="Input"/> sourced from a URL.
/// Implement this to control how remote content is fetched (for example to add caching via Replicant).
/// </summary>
public interface IPandocHttpClient
{
    Task<Stream> GetStream(string url, Cancel cancel = default);
}
