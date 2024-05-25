using FluentAssertions;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Tests.Validators;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

[TestFixture]
public class CreateMessageCommandValidatorTests
{
    private const string SampleChatId = "1";
    private const string SampleChatIdString = "6622e049321a5c22e5d8bb8d";
    private const string SampleImageId = "2";
    private const string SampleText = "Text";

    private CreateMessageCommandValidator _validator;
    private Mock<ISignPackageService> _mockSignPackageService;
    private Mock<IChatAgnosticService> _mockChatAgnosticService;

    [SetUp]
    public void SetUp()
    {
        _mockSignPackageService = new Mock<ISignPackageService>();
        _mockChatAgnosticService = new Mock<IChatAgnosticService>();
        _validator = new(_mockSignPackageService.Object, _mockChatAgnosticService.Object);
    }

    [Test]
    public async Task ValidateAsync_When_ModelIsEmpty_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand();
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();

        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Chat Id' must not be empty.",
            "'Chat Id' is required",
            "'Text' and 'ImageId' both cannot be empty"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsEmptyString_TextIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = string.Empty
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Chat Id' is required",
            "'Text' and 'ImageId' both cannot be empty"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [TestCase("0")]
    [TestCase("-1")]
    public async Task ValidateAsync_When_ChatIdIsInvalid_TextIsNull_ImageIsNull_ShouldContainValidationErrors(string chatId)
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = chatId
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Chat Id' must be greater than 0, when it is an integer",
            "'Text' and 'ImageId' both cannot be empty"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [TestCase(SampleChatId)]
    [TestCase(SampleChatIdString)]
    public async Task ValidateAsync_When_ChatIdIsValid_TextIsNull_ImageIsNull_ShouldContainValidationErrors(string chatId)
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = chatId
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(chatId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Text' and 'ImageId' both cannot be empty"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(chatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_DoesNotExistInDb_TextIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            $"Chat with 'Chat Id' = '{SampleChatId}' must exist",
            "'Text' and 'ImageId' both cannot be empty"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_TextIsEmpty_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            Text = string.Empty
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Text' and 'ImageId' both cannot be empty"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_TextIsNull_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            }
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Image' signature is invalid"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_TextIsValid_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            },
            Text = SampleText
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Image' signature is invalid"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsNull_TextIsNull_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            }
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Chat Id' must not be empty.",
            "'Chat Id' is required",
            "'Image' signature is invalid"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsNull_TextIsValid_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            },
            Text = SampleText
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "'Chat Id' must not be empty.",
            "'Chat Id' is required",
            "'Image' signature is invalid"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_TextIsNotEmpty_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            Text = SampleText
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_TextIsNull_ImageIsValid_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            }
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken))
            .ReturnsAsync(true);

        _mockSignPackageService
            .Setup(x => x.Validate(request.Image))
            .Returns(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValid_TextIsValid_ImageIsValid_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            },
            Text = SampleText
        };
        var cancellationToken = new CancellationToken();

        _mockChatAgnosticService
            .Setup(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken))
            .ReturnsAsync(true);

        _mockSignPackageService
            .Setup(x => x.Validate(request.Image))
            .Returns(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }
}
#pragma warning restore CA1861 // Avoid constant arrays as arguments