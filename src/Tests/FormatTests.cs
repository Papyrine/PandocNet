[NotInParallel]
public class FormatTests
{
    static readonly MethodInfo convertToText = typeof(PandocInstance)
        .GetMethods()
        .First(_ =>
            _.Name == "ConvertToText" &&
            _.GetParameters()[0].ParameterType == typeof(string));

    public static IEnumerable<Type> OutputFormats() =>
        typeof(OutOptions).Assembly
            .GetTypes()
            .Where(_ => _ is { IsAbstract: false, IsClass: true } &&
                        _.IsSubclassOf(typeof(OutOptions)))
            .OrderBy(_ => _.Name);

    public static IEnumerable<Type> InputFormats() =>
        typeof(InOptions).Assembly
            .GetTypes()
            .Where(_ => _ is { IsAbstract: false, IsClass: true } &&
                        _.IsSubclassOf(typeof(InOptions)))
            .OrderBy(_ => _.Name);

    [Test]

    [MethodDataSource(nameof(OutputFormats))]
    public async Task ConvertToOutput(Type outputType)
    {
        if (outputType == typeof(PdfOut))
        {
            Skip.Test("Requires PDF engine");
            return;
        }

        var method = convertToText.MakeGenericMethod(typeof(CommonMarkIn), outputType);
        var task = (Task<StringResult>) method.Invoke(null, ["*text*", null, null, null, Cancel.None])!;
        await task;
    }

    [Test]

    [MethodDataSource(nameof(InputFormats))]
    public async Task ConvertFromInput(Type inputType)
    {
        if (inputType == typeof(DocxIn) ||
            inputType == typeof(PptxIn) ||
            inputType == typeof(EpubIn) ||
            inputType == typeof(OdtIn) ||
            inputType == typeof(XlsxIn))
        {
            Skip.Test("Requires binary file input");
            return;
        }

        if (inputType == typeof(CslJsonIn) ||
            inputType == typeof(EndNoteXmlIn) ||
            inputType == typeof(Fib2In) ||
            inputType == typeof(HaskellIn) ||
            inputType == typeof(JsonIn) ||
            inputType == typeof(JupyterIn) ||
            inputType == typeof(RisIn) ||
            inputType == typeof(XmlIn))
        {
            Skip.Test("Requires structured input");
            return;
        }

        var method = convertToText.MakeGenericMethod(inputType, typeof(TxtOut));
        var task = (Task<StringResult>) method.Invoke(null, ["text", null, null, null, Cancel.None])!;
        await task;
    }
}
