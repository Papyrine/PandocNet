namespace Pandoc;

public class PandocEngine(string? pandocPath = null, Func<HttpClient>? httpClientFactory = null)
{
    internal string pandocPath = pandocPath ?? "pandoc";

    static readonly HttpClient defaultClient = new();

    HttpClient GetHttpClient() =>
        httpClientFactory?.Invoke() ?? defaultClient;

    public virtual async Task<StringResult> ConvertToText<TIn, TOut>(
        Input input,
        TIn? inOptions = null,
        TOut? outOptions = null,
        Options? options = null,
        Cancel cancel = default)
        where TIn : InOptions, new()
        where TOut : OutOptions, new()
    {
        var output = new StringBuilder();
        var command = await Convert(input, output, inOptions, outOptions, options, cancel);
        return new(command.Command, output.ToString());
    }

    public async Task<Result> Convert<TIn, TOut>(
        Input input,
        Output output,
        TIn? inOptions,
        TOut? outOptions,
        Options? options,
        Cancel cancel = default)
        where TIn : InOptions, new()
        where TOut : OutOptions, new()
    {
        inOptions ??= new();
        outOptions ??= new();
        var arguments = new List<string>(Options.GetArguments(options))
        {
            // Force binary to stdout
            "--output=-"
        };
        arguments.AddRange(inOptions.GetArguments());
        arguments.AddRange(outOptions.GetArguments());

        var command = CommandFormatter.Build(pandocPath, arguments);

        var startInfo = new ProcessStartInfo
        {
            FileName = pandocPath,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };
        process.Start();

        var inputTask = PipeInput(process, input, cancel);
        var outputTask = output.ReadFrom(process.StandardOutput.BaseStream, cancel);
        var errorTask = process.StandardError.ReadToEndAsync(cancel);

        await Task.WhenAll(inputTask, outputTask, errorTask);
        await process.WaitForExitAsync(cancel);

        CheckErrorCodes(process.ExitCode, await errorTask, command);
        return new(command);
    }

    async Task PipeInput(Process process, Input input, Cancel cancel)
    {
        var stream = process.StandardInput.BaseStream;
        try
        {
            await input.WriteTo(stream, GetHttpClient(), cancel);
            await stream.FlushAsync(cancel);
        }
        catch (IOException)
        {
            // pandoc closed stdin early (e.g. it errored before reading all input);
            // swallow the broken pipe so the real exit code surfaces below
        }

        process.StandardInput.Close();
    }

    static void CheckErrorCodes(int exitCode, string errors, string command)
    {
        if (exitCode == 0)
        {
            return;
        }

        var errorType = ErrorCodes.GetErrorType(exitCode);
        throw new(
            $"""
             {errorType} ({exitCode}):
             {command}
             {errors}
             """);
    }
}