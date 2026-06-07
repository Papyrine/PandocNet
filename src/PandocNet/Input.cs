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

    public async Task WriteTo(Stream destination, HttpClient client, Cancel cancel)
    {
        if (file != null)
        {
            await using var fileStream = File.OpenRead(file);
            await fileStream.CopyToAsync(destination, cancel);
            return;
        }

        if (stream != null)
        {
            await stream.CopyToAsync(destination, cancel);
            return;
        }

        if (bytes != null)
        {
            await destination.WriteAsync(bytes, cancel);
            return;
        }

        if (url != null)
        {
            await using var urlStream = await client.GetStreamAsync(url, cancel);
            await urlStream.CopyToAsync(destination, cancel);
            return;
        }

        if (content != null)
        {
            await destination.WriteAsync(Encoding.UTF8.GetBytes(content), cancel);
            return;
        }

        throw new("Unknown input");
    }
}