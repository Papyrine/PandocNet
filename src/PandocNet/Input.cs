namespace Pandoc;

public class Input
{
    string? file;
    string? url;
    string? content;
    byte[]? bytes;
    Stream? stream;

    public Input(string value)
    {
        if (value.StartsWith("http://") ||
            value.StartsWith("https://"))
        {
            url = value;
            return;
        }

        if (File.Exists(value))
        {
            file = value;
            return;
        }

        content = value;
    }

    public Input(Stream stream) =>
        this.stream = stream;

    public Input(byte[] bytes) =>
        this.bytes = bytes;

    public static implicit operator Input(string value) => new(value);
    public static implicit operator Input(Stream stream) => new(stream);
    public static implicit operator Input(byte[] bytes) => new(bytes);

    public PipeSource GetPipeSource(HttpClient client)
    {
        if (file != null)
        {
            return PipeSource.FromFile(file);
        }

        if (stream != null)
        {
            return PipeSource.FromStream(stream);
        }

        if (bytes != null)
        {
            return PipeSource.FromBytes(bytes);
        }

        if (url != null)
        {
            return PipeSource.Create(
                async (destination, cancellation) =>
                {
                    await using var stream = await client.GetStreamAsync(url, cancellation);
                    await stream.CopyToAsync(destination, cancellation);
                });
        }

        if (content != null)
        {
            return PipeSource.FromString(content, Encoding.UTF8);
        }

        throw new("Unknown output");
    }
}