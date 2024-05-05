using FluentAssertions;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Tests.Validators;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

[TestFixture]
public class CreateMessageCommandValidatorTests
{
    private const string SampleChatId = "1";
    private const string SampleSenderId = "2";
    private const string SampleImageId = "3";
    private const string SampleText = "Text";

    private CreateMessageCommandValidator _validator;
    private Mock<ISignPackageService> _mockSignPackageService;

    [SetUp]
    public void SetUp()
    {
        _mockSignPackageService = new Mock<ISignPackageService>();
        _validator = new(_mockSignPackageService.Object);
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
        validationResult.Errors.Should().HaveCount(4);

        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty",
            "Chat Id is required",
            "Sender Id is required",
            "'Sender Id' must not be empty."
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_ChatIdIsNull_SenderIsZero_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            SenderId = "0"
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(2);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty",
            "Chat Id is required"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_ChatIdIsZero_SenderIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = "0"
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(3);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty",
            "Sender Id is required",
            "'Sender Id' must not be empty."
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_ChatIdIsZero_SenderIsZero_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = "0",
            SenderId = "0"
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_ChatIdIsValid_SenderIsValid_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            SenderId = SampleSenderId
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsEmpty_ChatIdIsValid_SenderIsValid_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            SenderId = SampleSenderId,
            Text = string.Empty
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_ChatIdIsValid_SenderIsValid_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            SenderId = SampleSenderId,
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
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsValid_ChatIdIsValid_SenderIsValid_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            SenderId = SampleSenderId,
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
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_ChatIdIsNull_SenderIsNull_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
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
        validationResult.Errors.Should().HaveCount(4);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Chat Id is required",
            "Sender Id is required",
            "'Sender Id' must not be empty.",
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsValid_ChatIdIsNull_SenderIsNull_ImageIsValid_SignatureIsInvalid_ShouldContainValidationErrors()
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
        validationResult.Errors.Should().HaveCount(4);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Chat Id is required",
            "Sender Id is required",
            "'Sender Id' must not be empty.",
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNotEmpty_ChatIdIsValid_SenderIsValid_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            SenderId = SampleSenderId,
            Text = SampleText
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_ChatIdIsValid_SenderIsValid_ImageIsValid_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            SenderId = SampleSenderId,
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            }
        };
        var cancellationToken = new CancellationToken();
        _mockSignPackageService
            .Setup(x => x.Validate(request.Image))
            .Returns(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_TextIsValid_ChatIdIsValid_SenderIsValid_ImageIsValid_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId,
            SenderId = SampleSenderId,
            Image = new ImageRequestModel
            {
                Id = SampleImageId
            },
            Text = SampleText
        };
        var cancellationToken = new CancellationToken();
        _mockSignPackageService
            .Setup(x => x.Validate(request.Image))
            .Returns(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
    }
}
#pragma warning restore CA1861 // Avoid constant arrays as arguments