[TestFixture]
public class CommandFormatterTests
{
    [Test]
    public void SimpleArgumentsAreNotQuoted() =>
        Assert.That(
            CommandFormatter.Build("pandoc", ["--from=commonmark", "--to=html"]),
            Is.EqualTo("pandoc --from=commonmark --to=html"));

    [Test]
    public void NoArguments() =>
        Assert.That(
            CommandFormatter.Build("pandoc", []),
            Is.EqualTo("pandoc"));

    [Test]
    public void ArgumentWithSpaceIsQuoted() =>
        Assert.That(
            CommandFormatter.Build("pandoc", ["--data-dir=C:\\foo bar"]),
            Is.EqualTo("pandoc \"--data-dir=C:\\foo bar\""));

    [Test]
    public void EmptyArgumentIsQuoted() =>
        Assert.That(
            CommandFormatter.Build("pandoc", [""]),
            Is.EqualTo("pandoc \"\""));

    [Test]
    public void EmbeddedQuoteIsEscaped() =>
        Assert.That(
            CommandFormatter.Build("pandoc", ["a\"b"]),
            Is.EqualTo("pandoc \"a\\\"b\""));

    [Test]
    public void TrailingBackslashIsDoubledWhenQuoted() =>
        // space forces quoting; the trailing backslash must be doubled so it
        // is not read as escaping the closing quote
        Assert.That(
            CommandFormatter.Build("pandoc", ["a \\"]),
            Is.EqualTo("pandoc \"a \\\\\""));

    [Test]
    public void BackslashBeforeQuoteIsEscaped() =>
        Assert.That(
            CommandFormatter.Build("pandoc", ["a\\\"b"]),
            Is.EqualTo("pandoc \"a\\\\\\\"b\""));

    [Test]
    public void InteriorBackslashIsNotDoubled() =>
        Assert.That(
            CommandFormatter.Build("pandoc", ["a\\b c"]),
            Is.EqualTo("pandoc \"a\\b c\""));
}
