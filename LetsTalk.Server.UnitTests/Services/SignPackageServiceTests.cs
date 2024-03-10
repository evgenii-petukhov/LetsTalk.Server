using FluentAssertions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.SignPackage;
using LetsTalk.Server.SignPackage.Abstractions;
using LetsTalk.Server.UnitTests.Models;
using LetsTalk.Server.UnitTests.Models.Signable;
using LetsTalk.Server.UnitTests.TestCases;
using Microsoft.Extensions.Options;
using Moq;

namespace LetsTalk.Server.UnitTests.Services;

[TestFixture]
public class SignPackageServiceTests
{
    private SignPackageService _signPackageService;
    private Mock<IOptions<SignPackageSettings>> _mockSignPackageSettingsOptions;

    [SetUp]
    public void SetUp()
    {
        _mockSignPackageSettingsOptions = new Mock<IOptions<SignPackageSettings>>();
        _signPackageService = new SignPackageService(_mockSignPackageSettingsOptions.Object);
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
        action.Should().Throw<Exception>().Which.Message.Should().Be("There is no supported properties to sign");
    }

    [Test]
    [TestCaseSource(
        typeof(SignPackageServiceTestCases),
        nameof(SignPackageServiceTestCases.ObjectsToSign)
    )]
    public void Sign_When_ObjIsValid_ShouldReturnExpectedResult(TestData<ISignable, string> testData)
    {
        // Arrange
        // Act
        _signPackageService.Sign(testData.Value!);

        // Assert
        testData.Result.Should()
            .NotBeNull()
            .And.Be(testData.Result);
    }

    [Test]
    [TestCaseSource(
        typeof(SignPackageServiceTestCases),
        nameof(SignPackageServiceTestCases.ObjectsToValidate)
    )]
    public void Validate_ShouldReturnExpectedResult(TestData<SimpleSignable, bool> testData)
    {
        // Arrange
        // Act
        var result = _signPackageService.Validate(testData.Value!);

        // Assert
        result.Should().Be(testData.Result);
    }
}
