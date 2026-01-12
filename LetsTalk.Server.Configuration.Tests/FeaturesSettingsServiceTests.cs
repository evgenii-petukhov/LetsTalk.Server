using FluentAssertions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Configuration.Services;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;
using Moq;

namespace LetsTalk.Server.Configuration.Tests;

[TestFixture]
public class FeaturesSettingsServiceTests
{
    private Mock<IOptions<FeaturesSettings>> _optionsMock;
    private FeaturesSettingsService _service;

    [SetUp]
    public void SetUp()
    {
        _optionsMock = new Mock<IOptions<FeaturesSettings>>();
    }

    [Test]
    [TestCase("Local", FileStorageTypes.Local)]
    [TestCase("AmazonS3", FileStorageTypes.AmazonS3)]
    [TestCase("AzureBlobStorage", FileStorageTypes.AzureBlobStorage)]
    public void GetFileStorageType_When_ValidEnumValue_ShouldReturnCorrectType(string fileStorage, FileStorageTypes expected)
    {
        // Arrange
        var settings = new FeaturesSettings { FileStorage = fileStorage };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        _service = new FeaturesSettingsService(_optionsMock.Object);

        // Act
        var result = _service.GetFileStorageType();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [TestCase("1", FileStorageTypes.Local)]
    [TestCase("2", FileStorageTypes.AmazonS3)]
    [TestCase("3", FileStorageTypes.AzureBlobStorage)]
    public void GetFileStorageType_When_ValidNumericValue_ShouldReturnCorrectType(string fileStorage, FileStorageTypes expected)
    {
        // Arrange
        var settings = new FeaturesSettings { FileStorage = fileStorage };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        _service = new FeaturesSettingsService(_optionsMock.Object);

        // Act
        var result = _service.GetFileStorageType();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("InvalidValue")]
    public void GetFileStorageType_When_InvalidValue_ShouldReturnLocal(string fileStorage)
    {
        // Arrange
        var settings = new FeaturesSettings { FileStorage = fileStorage };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        _service = new FeaturesSettingsService(_optionsMock.Object);

        // Act
        var result = _service.GetFileStorageType();

        // Assert
        result.Should().Be(FileStorageTypes.Local);
    }

    [Test]
    [TestCase("local")]
    [TestCase("amazons3")]
    [TestCase("azureblobstorage")]
    public void GetFileStorageType_When_CaseInsensitiveValue_ShouldReturnLocal(string fileStorage)
    {
        // Arrange
        var settings = new FeaturesSettings { FileStorage = fileStorage };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        _service = new FeaturesSettingsService(_optionsMock.Object);

        // Act
        var result = _service.GetFileStorageType();

        // Assert
        result.Should().Be(FileStorageTypes.Local);
    }
}