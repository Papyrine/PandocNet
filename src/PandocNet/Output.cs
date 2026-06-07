namespace Pandoc;

public class Output
{
    string? file;
    StringBuilder? stringBuilder;
    Stream? stream;

    public Output(string file) =>
        this.file = file;

    public Output(Stream stream) =>
        this.stream = stream;

    public Output(StringBuilder stringBuilder) =>
        this.stringBuilder = stringBuilder;

    public static implicit operator Output(string value) => new(value);
    public static implicit operator Output(Stream stream) => new(stream);
    public static implicit operator Output(StringBuilder stringBuilder) => new(stringBuilder);

    public async Task ReadFrom(Stream source, Cancel cancel)
    {
        if (stringBuilder != null)
        {
            using var reader = new StreamReader(source, Encoding.UTF8);
            stringBuilder.Append(await reader.ReadToEndAsync(cancel));
            return;
        }

        if (file != null)
        {
            await using var fileStream = File.Create(file);
            await source.CopyToAsync(fileStream, cancel);
            return;
        }

        if (stream != null)
        {
            await source.CopyToAsync(stream, cancel);
            return;
        }

        throw new("Unknown output");
    }
}