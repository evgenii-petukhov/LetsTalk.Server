using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.UnitTests.Services.TestCases;

public class ValidateTestData
{
    public ISignable? Value { get; set; }

    public bool Result { get; set; }
}
