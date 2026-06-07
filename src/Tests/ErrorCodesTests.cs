[TestFixture]
public class ErrorCodesTests
{
    [TestCase(6, "PandocOptionError")]
    [TestCase(22, "PandocUnknownWriterError")]
    [TestCase(64, "PandocParseError")]
    [TestCase(99, "PandocResourceNotFound")]
    public void KnownCodesAreMapped(int exitCode, string expected) =>
        Assert.That(ErrorCodes.GetErrorType(exitCode), Is.EqualTo(expected));

    [Test]
    public void UnknownCodeFallsBack() =>
        Assert.That(ErrorCodes.GetErrorType(1234), Is.EqualTo("PandocUnknownError"));
}
