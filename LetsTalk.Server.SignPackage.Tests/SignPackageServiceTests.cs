using FluentAssertions;
using LetsTalk.Server.SignPackage.Models;
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
    public void Sign_NullObject_ShouldNotThrowException()
    {
        // Arrange

        // Act
        Action action = () => _signPackageService.Sign(null!);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void Sign_NonSignableObject_ShouldNotThrowException()
    {
        // Arrange

        // Act
        Action action = () => _signPackageService.Sign(0);

        // Assert
        action.Should().NotThrow();
    }

    [Test]
    public void Sign_ObjectWithNoSupportedProperties_ShouldThrowException()
    {
        // Arrange

        // Act
        Action action = () => _signPackageService.Sign(new NoPropertiesSignable());

        // Assert
        action.Should().Throw<Exception>().Which.Message.Should().Be("There are no supported properties to sign");
    }

    [Test]
    public void Sign_ValidObject_ShouldSetSignatureAndValidateSuccessfully()
    {
        // Arrange
        var signable = new SimpleSignable
        {
            A = 1,
            B = "B"
        };

        // Act
        _signPackageService.Sign(signable);
        var result = _signPackageService.Validate(signable);

        // Assert
        signable.Signature.Should()
            .NotBeNull();

        result.Should().BeTrue();
    }

    [Test]
    public void Sign_ValidObjectWithModifiedSignature_ShouldFailValidation()
    {
        // Arrange
        var signable = new SimpleSignable
        {
            A = 1,
            B = "B"
        };

        // Act
        _signPackageService.Sign(signable);
        signable.Signature += "1";
        var result = _signPackageService.Validate(signable);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    [TestCaseSource(
        typeof(SignPackageServiceTestCases),
        nameof(SignPackageServiceTestCases.ObjectsToSign)
    )]
    public void Sign_ValidObjectFromTestCaseSource_ShouldSetSignatureAndValidateSuccessfully(ISignable signable)
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
