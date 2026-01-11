using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Features.Message.Queries.GetMessages;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.CommandValidators;

[TestFixture]
public class GetMessagesQueryValidatorTests
{
    private Mock<IChatService> _chatServiceMock;
    private GetMessagesQueryValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _chatServiceMock = new Mock<IChatService>();
        _validator = new GetMessagesQueryValidator(_chatServiceMock.Object);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsValidAndUserIsMember_ShouldBeValid()
    {
        // Arrange
        var query = new GetMessagesQuery("senderId", "chatId", 0, 10);
        _chatServiceMock.Setup(x => x.IsChatIdValidAsync("chatId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _chatServiceMock.Setup(x => x.IsAccountChatMemberAsync("chatId", "senderId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsInvalid_ShouldContainValidationError()
    {
        // Arrange
        var query = new GetMessagesQuery("senderId", "chatId", 0, 10);
        _chatServiceMock.Setup(x => x.IsChatIdValidAsync("chatId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _chatServiceMock.Setup(x => x.IsAccountChatMemberAsync("chatId", "senderId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }

    [Test]
    public async Task ValidateAsync_When_UserIsNotChatMember_ShouldContainValidationError()
    {
        // Arrange
        var query = new GetMessagesQuery("senderId", "chatId", 0, 10);
        _chatServiceMock.Setup(x => x.IsChatIdValidAsync("chatId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _chatServiceMock.Setup(x => x.IsAccountChatMemberAsync("chatId", "senderId", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task ValidateAsync_When_ChatIdIsNullOrWhitespace_ShouldContainValidationError(string chatId)
    {
        // Arrange
        var query = new GetMessagesQuery("senderId", chatId, 0, 10);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        _chatServiceMock.Verify(x => x.IsChatIdValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}