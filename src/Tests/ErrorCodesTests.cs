[NotInParallel]
public class ErrorCodesTests
{
    [Test]
    [Arguments(6, "PandocOptionError")]
    [Arguments(22, "PandocUnknownWriterError")]
    [Arguments(64, "PandocParseError")]
    [Arguments(99, "PandocResourceNotFound")]
    public async Task KnownCodesAreMapped(int exitCode, string expected) =>
        await Assert.That(ErrorCodes.GetErrorType(exitCode)).IsEqualTo(expected);

    [Test]
    public async Task UnknownCodeFallsBack() =>
        await Assert.That(ErrorCodes.GetErrorType(1234)).IsEqualTo("PandocUnknownError");
}
