[NotInParallel]
public class OptionsTests
{
    [Test]
    public async Task OutVariablesJson()
    {
        var options = new HtmlOut
        {
            VariablesJson = new Dictionary<string, string>
            {
                { "foo", "false" },
                { "colors", "[\"red\",\"green\"]" }
            }
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--variable-json=foo:false");
        await Assert.That(args).Contains("--variable-json=colors:[\"red\",\"green\"]");
    }

    [Test]
    public async Task OutMetadata()
    {
        var options = new HtmlOut
        {
            Metadata = new Dictionary<string, string>
            {
                { "title", "My Document" },
                { "author", "Jane" }
            }
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--metadata=title:My Document");
        await Assert.That(args).Contains("--metadata=author:Jane");
    }

    [Test]
    public async Task OutMetadataFile()
    {
        var options = new HtmlOut
        {
            MetadataFile = "meta.yaml"
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--metadata-file=meta.yaml");
    }

    [Test]
    public async Task OutVariables()
    {
        var options = new HtmlOut
        {
            Variables = new Dictionary<string, string>
            {
                { "margin-top", "1in" }
            }
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--variable=margin-top:1in");
    }

    [Test]
    public async Task InMetadata()
    {
        var options = new CommonMarkIn
        {
            Metadata = new Dictionary<string, string>
            {
                { "title", "Test" },
                { "draft", "true" }
            }
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--metadata=title:Test");
        await Assert.That(args).Contains("--metadata=draft:true");
    }

    [Test]
    public async Task InMetadataFile()
    {
        var options = new CommonMarkIn
        {
            MetadataFile = "metadata.yaml"
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--metadata-file=metadata.yaml");
    }

    [Test]
    public async Task InFileScope()
    {
        var options = new CommonMarkIn
        {
            FileScope = true
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--file-scope");
    }

    [Test]
    public async Task InTrace()
    {
        var options = new CommonMarkIn
        {
            Trace = true
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--trace");
    }

    [Test]
    public async Task GlobalVerbose()
    {
        var args = Options.GetArguments(new Options { Verbose = true }).ToList();

        await Assert.That(args).Contains("--verbose");
    }

    [Test]
    public async Task GlobalQuiet()
    {
        var args = Options.GetArguments(new Options { Quiet = true }).ToList();

        await Assert.That(args).Contains("--quiet");
    }

    [Test]
    public async Task GlobalFailIfWarnings()
    {
        var args = Options.GetArguments(new Options { FailIfWarnings = true }).ToList();

        await Assert.That(args).Contains("--fail-if-warnings");
    }

    [Test]
    public async Task OutListOfFigures()
    {
        var options = new HtmlOut
        {
            ListOfFigures = true
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--list-of-figures");
    }

    [Test]
    public async Task OutListOfTables()
    {
        var options = new HtmlOut
        {
            ListOfTables = true
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--list-of-tables");
    }

    [Test]
    public async Task OutFigureCaptionPosition()
    {
        var options = new HtmlOut
        {
            FigureCaptionPosition = CaptionPosition.Above
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--figure-caption-position=above");
    }

    [Test]
    public async Task OutTableCaptionPosition()
    {
        var options = new HtmlOut
        {
            TableCaptionPosition = CaptionPosition.Below
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--table-caption-position=below");
    }

    [Test]
    public async Task OutSyntaxHighlighting()
    {
        var options = new HtmlOut
        {
            SyntaxHighlighting = "kate"
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--syntax-highlighting=kate");
    }

    [Test]
    public async Task HtmlEmailObfuscation()
    {
        var options = new HtmlOut
        {
            EmailObfuscation = EmailObfuscation.Javascript
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--email-obfuscation=javascript");
    }

    [Test]
    public async Task RstListTables()
    {
        var options = new RstOut
        {
            ListTables = true
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--list-tables");
    }

    [Test]
    public async Task PdfEngineOpt()
    {
        var options = new PdfOut
        {
            Engine = PdfEngine.XeLatex,
            EngineOpt = "--shell-escape"
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--pdf-engine=xelatex");
        await Assert.That(args).Contains("--pdf-engine-opt=--shell-escape");
    }

    [Test]
    public async Task EpubSplitLevel()
    {
        var options = new Epub3Out
        {
            SplitLevel = 2
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--split-level=2");
    }

    [Test]
    public async Task EpubTitlePage()
    {
        var options = new Epub3Out
        {
            EpubTitlePage = false
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--epub-title-page=false");
    }

    [Test]
    public async Task ChunkedHtmlSplitLevel()
    {
        var options = new ChunkedHtmlOut
        {
            SplitLevel = 3
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--split-level=3");
    }

    [Test]
    public async Task ChunkedHtmlChunkTemplate()
    {
        var options = new ChunkedHtmlOut
        {
            ChunkTemplate = "%s-%i.html"
        };

        var args = options.GetArguments().ToList();

        await Assert.That(args).Contains("--chunk-template=%s-%i.html");
    }
}
