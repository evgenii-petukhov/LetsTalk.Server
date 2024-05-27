using FluentAssertions;
using LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Tests.Validators.CommandValidators;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

[TestFixture]
public class CreateMessageCommandValidatorTests
{
    private const string SampleChatId = "1";

    private CreateMessageCommandValidator _validator;
    private Mock<IChatAgnosticService> _mockChatAgnosticService;

    [SetUp]
    public void SetUp()
    {
        _mockChatAgnosticService = new Mock<IChatAgnosticService>();
        _validator = new(_mockChatAgnosticService.Object);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_TextIsNull_ImageIsNull_DoesNotExistInDatabase_ShouldContainValidationErrors()
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
            "Chat must exist"
        });
        _mockChatAgnosticService.Verify(x => x.IsChatIdValidAsync(SampleChatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidInteger_TextIsNotEmpty_ImageIsNull_ExistsInDatabase_ShouldBeValid()
    {
        // Arrange
        var request = new CreateMessageCommand
        {
            ChatId = SampleChatId
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
}
#pragma warning restore CA1861 // Avoid constant arrays as arguments