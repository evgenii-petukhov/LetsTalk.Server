using FluentAssertions;
using LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;
using LetsTalk.Server.Persistence.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Validators;

[TestFixture]
public class CreateMessageCommandValidatorTests
{
    private CreateMessageCommandValidator _validator;
    private Mock<IAccountRepository> _mockAccountRepository;

    [SetUp]
    public void SetUp()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _validator = new(_mockAccountRepository.Object);
    }

    [Test]
    public async Task CreateMessageCommandValidator_EmptyModel()
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
            "Text is required",
            "Text cannot be empty",
            "Recipient Id is required",
            "Sender Id is required"
        });
    }

    [Test]
    public async Task CreateMessageCommandValidator_TextIsNull_RecipientIsNull_SenderIsZero()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            SenderId = 0
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
            "Text is required",
            "Text cannot be empty",
            "Recipient Id is required"
        });
    }

    [Test]
    public async Task CreateMessageCommandValidator_TextIsNull_RecipientIsZero_SenderIsNull()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = 0
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0, cancellationToken))
            .Returns(Task.FromResult(false));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(4);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text is required",
            "Text cannot be empty",
            "Sender Id is required",
            "Account with Recipient Id = 0 does not exist"
        });
    }

    [Test]
    public async Task CreateMessageCommandValidator_TextIsNull_RecipientIsZero_SenderIsZero()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = 0,
            SenderId = 0
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0, cancellationToken))
            .Returns(Task.FromResult(false));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(4);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text is required",
            "Text cannot be empty",
            "Recipient Id can't be equal to Sender Id",
            "Account with Recipient Id = 0 does not exist"
        });
    }

    [Test]
    public async Task CreateMessageCommandValidator_TextIsNull_RecipientIsNotNull_SenderIsNotNull_AccountDoesNotExist()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = 1,
            SenderId = 2
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(1, cancellationToken))
            .Returns(Task.FromResult(false));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(3);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text is required",
            "Text cannot be empty",
            "Account with Recipient Id = 1 does not exist"
        });
    }

    [Test]
    public async Task CreateMessageCommandValidator_TextIsNull_RecipientIsNotNull_SenderIsNotNull_AccountExists()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = 1,
            SenderId = 2
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(1, cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(2);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text is required",
            "Text cannot be empty"
        });
    }

    [Test]
    public async Task CreateMessageCommandValidator_TextIsEmpty_RecipientIsNotNull_SenderIsNotNull_AccountExists()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = 1,
            SenderId = 2,
            Text = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(1, cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Text cannot be empty"
        });
    }

    [Test]
    public async Task CreateMessageCommandValidator_TextIsNotEmpty_RecipientIsNotNull_SenderIsNotNull_AccountExists()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            RecipientId = 1,
            SenderId = 2,
            Text = "text"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(1, cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
