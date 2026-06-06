namespace Pandoc;

public class PandocEngine(string? pandocPath = null, IPandocHttpClient? httpClient = null)
{
    internal string pandocPath = pandocPath ?? "pandoc";
    IPandocHttpClient httpClient = httpClient ?? new DefaultPandocHttpClient();

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
        var source = input.GetPipeSource(httpClient);
        var target = output.GetPipeTarget();
        var errors = new StringBuilder();
        var arguments = new List<string>(Options.GetArguments(options))
        {
            // Force binary to stdout
            "--output=-"
        };
        arguments.AddRange(inOptions.GetArguments());
        arguments.AddRange(outOptions.GetArguments());
        var command = Cli.Wrap(pandocPath)
            .WithArguments(arguments)
            .WithStandardOutputPipe(target)
            .WithStandardInputPipe(source)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(errors))
            .WithValidation(CommandResultValidation.None);

        var result = await command.ExecuteAsync(cancel);
        CheckErrorCodes(result, errors, command);
        return new(command.ToString());
    }

    static void CheckErrorCodes(CommandResult result, StringBuilder errors, Command command)
    {
        var exitCode = result.ExitCode;

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