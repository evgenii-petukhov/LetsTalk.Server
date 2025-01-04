using FluentAssertions;
using LetsTalk.Server.SignPackage.Models;
using LetsTalk.Server.SignPackage.Tests.Models;
using LetsTalk.Server.SignPackage.Tests.Models.Signable;
using LetsTalk.Server.SignPackage.Tests.TestCases;

namespace LetsTalk.Server.SignPackage.Tests;

[TestFixture]
public class SignPackageServiceTests
{
    private SignPackageService _signPackageService;

    [SetUp]
    public void SetUp()
    {
        _signPackageService = new SignPackageService();
    }

    [Test]
    public void Sign_When_ObjIsNull_ShowldNotThrowException()
    {
        // Arrange
        // Act
        Action action = () => _signPackageService.Sign(null!);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void Sign_When_ObjIsNotSignable_ShowldNotThrowException()
    {
        // Arrange
        // Act
        Action action = () => _signPackageService.Sign(0);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void Sign_When_ObjIsNotNull_NoSupportedProperties_ExceptionIsThrown()
    {
        // Arrange
        // Act
        Action action = () => _signPackageService.Sign(new NoPropertiesSignable());

        // Assert
        action.Should().Throw<Exception>().Which.Message.Should().Be("There are no supported properties to sign");
    }

    [Test]
    [TestCaseSource(
        typeof(SignPackageServiceTestCases),
        nameof(SignPackageServiceTestCases.ObjectsToSign)
    )]
    public void Sign_When_ObjIsValid_ShouldReturnExpectedResult(ISignable signable)
    {
        // Arrange
        // Act
        _signPackageService.Sign(signable);
        var result = _signPackageService.Validate(signable);

        // Assert
        signable.Signature.Should()
            .NotBeNull();

        result.Should().BeTrue();
    }
}
