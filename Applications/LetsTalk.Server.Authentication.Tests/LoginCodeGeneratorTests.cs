using FluentAssertions;
using LetsTalk.Server.Authentication.Services.Cache.LoginCodes;

namespace LetsTalk.Server.Authentication.Tests;

[TestFixture]
public class LoginCodeGeneratorTests
{
    private LoginCodeGenerator _loginCodeGenerator;

    [SetUp]
    public void SetUp()
    {
        _loginCodeGenerator = new LoginCodeGenerator();
    }

    [Test]
    public void GenerateCode_ShouldReturnCodeWithinRange()
    {
        // Act
        int code = _loginCodeGenerator.GenerateCode();

        // Assert
        code.Should().BeInRange(1000, 9999);
    }

    [Test]
    public void GenerateCode_ShouldReturnDifferentCodesOnSubsequentCalls()
    {
        // Act
        int code1 = _loginCodeGenerator.GenerateCode();
        int code2 = _loginCodeGenerator.GenerateCode();

        // Assert
        code1.Should().NotBe(code2);
    }

    [Test]
    public void GenerateCode_ShouldReturnValidCodesForMultipleGenerations()
    {
        // Arrange
        const int numberOfCodes = 1000;

        var codes = new List<int>(numberOfCodes);

        // Act
        for (int i = 0; i < numberOfCodes; i++)
        {
            codes.Add(_loginCodeGenerator.GenerateCode());
        }

        // Assert
        codes.Should().OnlyContain(code => code >= 1000 && code <= 9999);
    }
}
