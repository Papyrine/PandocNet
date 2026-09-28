[NotInParallel]
public class CommandFormatterTests
{
    [Test]
    public async Task SimpleArgumentsAreNotQuoted() =>
        await Assert.That(CommandFormatter.Build("pandoc", ["--from=commonmark", "--to=html"])).IsEqualTo("pandoc --from=commonmark --to=html");

    [Test]
    public async Task NoArguments() =>
        await Assert.That(CommandFormatter.Build("pandoc", [])).IsEqualTo("pandoc");

    [Test]
    public async Task ArgumentWithSpaceIsQuoted() =>
        await Assert.That(CommandFormatter.Build("pandoc", ["--data-dir=C:\\foo bar"])).IsEqualTo("pandoc \"--data-dir=C:\\foo bar\"");

    [Test]
    public async Task EmptyArgumentIsQuoted() =>
        await Assert.That(CommandFormatter.Build("pandoc", [""])).IsEqualTo("pandoc \"\"");

    [Test]
    public async Task EmbeddedQuoteIsEscaped() =>
        await Assert.That(CommandFormatter.Build("pandoc", ["a\"b"])).IsEqualTo("pandoc \"a\\\"b\"");

    [Test]
    public async Task TrailingBackslashIsDoubledWhenQuoted() =>
        // space forces quoting; the trailing backslash must be doubled so it
        // is not read as escaping the closing quote
        await Assert.That(CommandFormatter.Build("pandoc", ["a \\"])).IsEqualTo("pandoc \"a \\\\\"");

    [Test]
    public async Task BackslashBeforeQuoteIsEscaped() =>
        await Assert.That(CommandFormatter.Build("pandoc", ["a\\\"b"])).IsEqualTo("pandoc \"a\\\\\\\"b\"");

    [Test]
    public async Task InteriorBackslashIsNotDoubled() =>
        await Assert.That(CommandFormatter.Build("pandoc", ["a\\b c"])).IsEqualTo("pandoc \"a\\b c\"");
}
