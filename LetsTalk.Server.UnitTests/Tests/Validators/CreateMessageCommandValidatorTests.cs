﻿using FluentAssertions;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Tests.Validators;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

[TestFixture]
public class CreateMessageCommandValidatorTests
{
    private CreateMessageCommandValidator _validator;
    private Mock<IAccountAgnosticService> _mockAccountAgnosticService;
    private Mock<ISignPackageService> _mockSignPackageService;

    [SetUp]
    public void SetUp()
    {
        _mockAccountAgnosticService = new Mock<IAccountAgnosticService>();
        _mockSignPackageService = new Mock<ISignPackageService>();
        _validator = new(_mockAccountAgnosticService.Object, _mockSignPackageService.Object);
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
        validationResult.Errors.Should().HaveCount(3);

        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty",
            "Recipient Id is required",
            "Sender Id is required"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_RecipientIsNull_SenderIsZero_ImageIsNull_ShouldContainValidationErrors()
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
            "Recipient Id is required"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_RecipientIsZero_SenderIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "0"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(false));

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
            "Account with Recipient Id = 0 does not exist"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_RecipientIsZero_SenderIsZero_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "0",
            SenderId = "0"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(false));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(3);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty",
            "Recipient Id can't be equal to Sender Id",
            "Account with Recipient Id = 0 does not exist"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_RecipientIsNotNull_SenderIsNotNull_AccountDoesNotExist_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(false));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(2);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text and ImageId both cannot be empty",
            "Account with Recipient Id = 1 does not exist"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_RecipientIsNotNull_SenderIsNotNull_AccountExists_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));

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
    public async Task ValidateAsync_When_TextIsEmpty_RecipientIsNotNull_SenderIsNotNull_AccountExists_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2",
            Text = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));

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
    public async Task ValidateAsync_When_TextIsNull_RecipientIsNotNull_SenderIsNotNull_AccountExists_ImageIsNotNull_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2",
            Image = new ImageRequestModel
            {
                Id = "1"
            }
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));

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
    public async Task ValidateAsync_When_TextIsNotNull_RecipientIsNotNull_SenderIsNotNull_AccountExists_ImageIsNotNull_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2",
            Image = new ImageRequestModel
            {
                Id = "1"
            },
            Text = "text"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));

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
    public async Task ValidateAsync_When_TextIsNull_RecipientIsNull_SenderIsNull_AccountExists_ImageIsNotNull_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            Image = new ImageRequestModel
            {
                Id = "1"
            }
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(3);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Recipient Id is required",
            "Sender Id is required",
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNotNull_RecipientIsNull_SenderIsNull_AccountExists_ImageIsNotNull_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            Image = new ImageRequestModel
            {
                Id = "1"
            },
            Text = "text"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(3);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Recipient Id is required",
            "Sender Id is required",
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNotEmpty_RecipientIsNotNull_SenderIsNotNull_AccountExists_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2",
            Text = "text"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_TextIsNull_RecipientIsNotNull_SenderIsNotNull_AccountExists_ImageIsNotNull_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2",
            Image = new ImageRequestModel
            {
                Id = "1"
            }
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));
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
    public async Task ValidateAsync_When_TextIsNotNull_RecipientIsNotNull_SenderIsNotNull_AccountExists_ImageIsNotNull_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = "1",
            SenderId = "2",
            Image = new ImageRequestModel
            {
                Id = "1"
            },
            Text = "text"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("1", cancellationToken))
            .Returns(Task.FromResult(true));
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