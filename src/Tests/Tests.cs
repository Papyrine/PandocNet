[TestFixture]
public class Tests
{
    [Test]
    public void InputsAndOutputs()
    {
        WriteInclude("Input");
        WriteInclude("Output");
    }

    static void WriteInclude(string target)
    {
        var solutionDirectory = ProjectFiles.SolutionDirectory;
        var includePath = Path.Combine(solutionDirectory, $"{target.ToLower()}.include.md");
        var builder = new StringBuilder();
        var projectDirectory = Path.Combine(solutionDirectory, "PandocNet");
        var targetDirectory = Path.Combine(projectDirectory, target);
        foreach (var file in Directory.EnumerateFiles(targetDirectory, "*", SearchOption.AllDirectories))
        {
            var fileContent = File.ReadAllText(file)
                .Replace("namespace Pandoc;", "")
                .Trim();
            builder.AppendLine(
                $"""
                 #### {Path.GetFileNameWithoutExtension(file)}

                 ```cs
                 {fileContent}
                 ```


                 """);
        }

        File.Delete(includePath);
        File.WriteAllText(includePath, builder.ToString());
    }

    [Test]
    [Explicit]
    public async Task BinaryToText()
    {
        var result = await PandocInstance.Convert<DocxIn, HtmlOut>("sample.docx", "output.html");

        await VerifyFile("output.html")
            .AppendValue("command", result.Command);
    }

    [Test]
    public async Task DataDirectory()
    {
        var result = await PandocInstance.Convert<CommonMarkIn, HtmlOut>(
            "sample.md",
            "output.html",
            options: new()
            {
                DataDirectory = Environment.CurrentDirectory
            });

        await VerifyFile("output.html")
            .AppendValue("command", result.Command);
    }

    [Test]
    public async Task Files()
    {
        var result = await PandocInstance.Convert<CommonMarkIn, HtmlOut>("sample.md", "output.html");

        await VerifyFile("output.html")
            .AppendValue("command", result.Command);
    }

    [Test]
    public async Task Streams()
    {
        var result = await PandocInstance.Convert<CommonMarkIn, HtmlOut>("sample.md", "output.html");

        await VerifyFile("output.html")
            .AppendValue("command", result.Command);
    }

    [Test]
    public async Task InputOutput()
    {
        Pandoc.Result result;
        {
            await using var inStream = File.OpenRead("sample.md");
            await using var outStream = File.OpenWrite("output.html");
            result = await PandocInstance.Convert<CommonMarkIn, HtmlOut>(inStream, outStream);
        }

        await VerifyFile("output.html")
            .AppendValue("command", result.Command);
    }

    [Test]
    public async Task Text()
    {
        var (command, value) = await PandocInstance.ConvertToText<CommonMarkIn, HtmlOut>("*text*");

        await Verify(value, "html")
            .AppendValue("command", command);
    }

    [Test]
    public async Task TextFromStream()
    {
        using var stream = new MemoryStream("*text*"u8.ToArray());
        stream.Position = 0;

        var (command, value) = await PandocInstance.ConvertToText<CommonMarkIn, HtmlOut>(stream);

        await Verify(value, "html")
            .AppendValue("command", command);
    }

    [Test]
    public async Task TextFromBytes()
    {
        var bytes = "*text*"u8.ToArray();

        var (command, value) = await PandocInstance.ConvertToText<CommonMarkIn, HtmlOut>(bytes);

        await Verify(value, "html")
            .AppendValue("command", command);
    }

    [Test]
    public void ErrorCodeThrows()
    {
        var exception = Assert.ThrowsAsync<Exception>(
            () => PandocInstance.ConvertToText<JsonIn, HtmlOut>("not valid json"));

        // CheckErrorCodes formats the mapped error type, the command, and stderr
        Assert.That(exception!.Message, Does.Contain("pandoc --output=- --from=json --to=html"));
    }

    [Test]
    [Explicit]
    public async Task CustomOptions()
    {
        var (command, value) = await PandocInstance.ConvertToText(
            "# Heading1",
            new CommonMarkIn
            {
                ShiftHeadingLevelBy = 2
            },
            new HtmlOut
            {
                NumberOffsets = [3]
            });

        await Verify(value, "html")
            .AppendValue("command", command);
    }

    [Test]
    public async Task EncodingTest()
    {
        var (command, value) = await PandocInstance.ConvertToText(
            "白日依山尽，黄河入海流。欲穷千里目，更上一层楼。",
            new HtmlIn(),
            new CommonMarkOut());

        await Verify(value, "md")
            .AppendValue("command", command);
    }
}