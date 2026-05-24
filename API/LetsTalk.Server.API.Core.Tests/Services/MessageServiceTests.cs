using FluentAssertions;
using LetsTalk.Server.API.Core.Services;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Services;

[TestFixture]
public class MessageServiceTests
{
    private Mock<IMessageAgnosticService> _messageAgnosticServiceMock;
    private MessageService _messageService;

    [SetUp]
    public void SetUp()
    {
        _messageAgnosticServiceMock = new Mock<IMessageAgnosticService>();
        _messageService = new MessageService(_messageAgnosticServiceMock.Object);
    }

    [Test]
    public async Task GetPagedAsync_ShouldReturnMessages_WhenMessagesExist()
    {
        // Arrange
        var chatId = "chat123";
        var pageIndex = 0;
        var messagesPerPage = 10;
        var cancellationToken = CancellationToken.None;
        var expectedMessages = new List<MessageServiceModel>
        {
            new() { Id = "msg1", Text = "Hello", SenderId = "user1" },
            new() { Id = "msg2", Text = "World", SenderId = "user2" }
        };

        _messageAgnosticServiceMock
            .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken))
            .ReturnsAsync(expectedMessages);

        // Act
        var result = await _messageService.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedMessages);
        _messageAgnosticServiceMock.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetPagedAsync_ShouldReturnEmptyList_WhenNoMessagesExist()
    {
        // Arrange
        var chatId = "chat123";
        var pageIndex = 0;
        var messagesPerPage = 10;
        var cancellationToken = CancellationToken.None;
        var emptyMessages = new List<MessageServiceModel>();

        _messageAgnosticServiceMock
            .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken))
            .ReturnsAsync(emptyMessages);

        // Act
        var result = await _messageService.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken);

        // Assert
        result.Should().BeEmpty();
        _messageAgnosticServiceMock.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetPagedAsync_ShouldPassAllParameters_ToAgnosticService()
    {
        // Arrange
        var chatId = "chat456";
        var pageIndex = 2;
        var messagesPerPage = 25;
        var cancellationToken = new CancellationToken(true);
        var messages = new List<MessageServiceModel>();

        _messageAgnosticServiceMock
            .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken))
            .ReturnsAsync(messages);

        // Act
        await _messageService.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken);

        // Assert
        _messageAgnosticServiceMock.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken), Times.Once);
    }
}