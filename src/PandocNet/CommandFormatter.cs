namespace Pandoc;

static class CommandFormatter
{
    public static string Build(string path, IReadOnlyList<string> arguments)
    {
        var builder = new StringBuilder(path);
        foreach (var argument in arguments)
        {
            builder.Append(' ');
            AppendArgument(builder, argument);
        }

        return builder.ToString();
    }

    static void AppendArgument(StringBuilder builder, string argument)
    {
        if (argument.Length != 0 &&
            !argument.Any(_ => char.IsWhiteSpace(_) || _ == '"'))
        {
            builder.Append(argument);
            return;
        }

        builder.Append('"');
        for (var index = 0; index < argument.Length;)
        {
            var current = argument[index++];
            if (current == '\\')
            {
                var backslashCount = 1;
                while (index < argument.Length &&
                       argument[index] == '\\')
                {
                    backslashCount++;
                    index++;
                }

                if (index == argument.Length)
                {
                    builder.Append('\\', backslashCount * 2);
                }
                else if (argument[index] == '"')
                {
                    builder.Append('\\', backslashCount * 2 + 1);
                    builder.Append('"');
                    index++;
                }
                else
                {
                    builder.Append('\\', backslashCount);
                }
            }
            else if (current == '"')
            {
                builder.Append('\\');
                builder.Append('"');
            }
            else
            {
                builder.Append(current);
            }
        }

        builder.Append('"');
    }
}
