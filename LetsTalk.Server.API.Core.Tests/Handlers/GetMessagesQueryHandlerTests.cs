using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Features.Message.Queries.GetMessages;
using LetsTalk.Server.API.Middleware.Exceptions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using MediatR;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class GetMessagesQueryHandlerTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<IMessageService> _messageServiceMock;
    private Mock<IChatService> _chatServiceMock;
    private GetMessagesQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mapperMock = new Mock<IMapper>();
        _messageServiceMock = new Mock<IMessageService>();
        _chatServiceMock = new Mock<IChatService>();

        _handler = new GetMessagesQueryHandler(
            _mapperMock.Object,
            _messageServiceMock.Object,
            _chatServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenValidationSucceedsAndMessagesExist_ShouldReturnMappedMessages()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-123", ChatId = "chat-456", Text = "Message 1" },
            new() { Id = "msg-2", SenderId = "sender-456", ChatId = "chat-456", Text = "Message 2" },
            new() { Id = "msg-3", SenderId = "sender-123", ChatId = "chat-456", Text = "Message 3" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", Text = "Message 1", IsMine = false },
            new() { Id = "msg-2", Text = "Message 2", IsMine = false },
            new() { Id = "msg-3", Text = "Message 3", IsMine = false }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        
        result[0].Should().BeEquivalentTo(messageDtos[0] with { IsMine = true });
        result[1].Should().BeEquivalentTo(messageDtos[1] with { IsMine = false });
        result[2].Should().BeEquivalentTo(messageDtos[2] with { IsMine = true });

        VerifyAllInteractions(query, messageServiceModels, cancellationToken);
    }

    [Test]
    public async Task Handle_WhenNoMessagesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var emptyMessageList = new List<MessageServiceModel>();

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, emptyMessageList, cancellationToken);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _messageServiceMock.Verify(
            x => x.GetPagedAsync("chat-456", 0, 10, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MessageDto>(It.IsAny<MessageServiceModel>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenValidationFails_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        SetupValidationFailure(query, cancellationToken);

        // Act & Assert
        var exception = Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(query, cancellationToken));

        exception.Message.Should().Be("Chat with id 'chat-456' was not found.");

        _messageServiceMock.Verify(
            x => x.GetPagedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<MessageDto>(It.IsAny<MessageServiceModel>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_WhenAllMessagesAreMine_ShouldSetIsMineToTrue()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-123", ChatId = "chat-456", Text = "My Message 1" },
            new() { Id = "msg-2", SenderId = "sender-123", ChatId = "chat-456", Text = "My Message 2" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", Text = "My Message 1", IsMine = false },
            new() { Id = "msg-2", Text = "My Message 2", IsMine = false }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(msg => msg.IsMine.Should().BeTrue());
    }

    [Test]
    public async Task Handle_WhenNoMessagesAreMine_ShouldSetIsMineToFalse()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-456", ChatId = "chat-456", Text = "Other Message 1" },
            new() { Id = "msg-2", SenderId = "sender-789", ChatId = "chat-456", Text = "Other Message 2" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", Text = "Other Message 1", IsMine = true },
            new() { Id = "msg-2", Text = "Other Message 2", IsMine = true }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(msg => msg.IsMine.Should().BeFalse());
    }

    [Test]
    public async Task Handle_WhenDifferentPageParameters_ShouldPassCorrectParameters()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 5, 25);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-123", ChatId = "chat-456" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", IsMine = false }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _messageServiceMock.Verify(
            x => x.GetPagedAsync("chat-456", 5, 25, cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenMessageServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        SetupValidationSuccess(query, cancellationToken);

        _messageServiceMock
            .Setup(x => x.GetPagedAsync("chat-456", 0, 10, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Message service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, cancellationToken));

        _mapperMock.Verify(
            x => x.Map<MessageDto>(It.IsAny<MessageServiceModel>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenMapperThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-123", ChatId = "chat-456" }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(messageServiceModels[0]))
            .Throws(new InvalidOperationException("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToAllServices()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = new CancellationToken(false);

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-123", ChatId = "chat-456" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", IsMine = false }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert - Verification is done in the setup methods that check cancellationToken
        _messageServiceMock.Verify(
            x => x.GetPagedAsync("chat-456", 0, 10, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullSenderIdInMessage_ShouldSetIsMineToFalse()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = null, ChatId = "chat-456", Text = "Message with null sender" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", Text = "Message with null sender", IsMine = true }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result[0].IsMine.Should().BeFalse();
    }

    [Test]
    public async Task Handle_WhenEmptySenderIdInMessage_ShouldSetIsMineToFalse()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "", ChatId = "chat-456", Text = "Message with empty sender" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", Text = "Message with empty sender", IsMine = true }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result[0].IsMine.Should().BeFalse();
    }

    [Test]
    public async Task Handle_WhenNullQuerySenderId_ShouldSetIsMineCorrectly()
    {
        // Arrange
        var query = new GetMessagesQuery(null!, "chat-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-123", ChatId = "chat-456" },
            new() { Id = "msg-2", SenderId = null, ChatId = "chat-456" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", IsMine = true },
            new() { Id = "msg-2", IsMine = true }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        // When query.SenderId is null:
        // - message with SenderId = "sender-123" should have IsMine = false (null != "sender-123")
        // - message with SenderId = null should have IsMine = true (null == null)
        result[0].IsMine.Should().BeFalse();
        result[1].IsMine.Should().BeTrue();
    }

    [Test]
    public async Task Handle_WhenSpecialCharactersInIds_ShouldHandleCorrectly()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-!@#$%", "chat-^&*()", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "sender-!@#$%", ChatId = "chat-^&*()" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", IsMine = false }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result[0].IsMine.Should().BeTrue();
    }

    [Test]
    public async Task Handle_WhenUnicodeCharactersInIds_ShouldHandleCorrectly()
    {
        // Arrange
        var query = new GetMessagesQuery("发送者-123", "聊天-456", 0, 10);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>
        {
            new() { Id = "msg-1", SenderId = "发送者-123", ChatId = "聊天-456" },
            new() { Id = "msg-2", SenderId = "مرسل-789", ChatId = "聊天-456" }
        };

        var messageDtos = new List<MessageDto>
        {
            new() { Id = "msg-1", IsMine = false },
            new() { Id = "msg-2", IsMine = false }
        };

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result[0].IsMine.Should().BeTrue();
        result[1].IsMine.Should().BeFalse();
    }

    [Test]
    public async Task Handle_WhenLargePageSize_ShouldHandleCorrectly()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 0, 1000);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>();
        var messageDtos = new List<MessageDto>();

        // Create a large number of messages
        for (int i = 0; i < 500; i++)
        {
            messageServiceModels.Add(new MessageServiceModel 
            { 
                Id = $"msg-{i}", 
                SenderId = i % 2 == 0 ? "sender-123" : "other-sender", 
                ChatId = "chat-456" 
            });
            messageDtos.Add(new MessageDto { Id = $"msg-{i}", IsMine = false });
        }

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);
        SetupMapperSuccess(messageServiceModels, messageDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().HaveCount(500);
        
        // Verify IsMine is set correctly for alternating senders
        for (int i = 0; i < 500; i++)
        {
            result[i].IsMine.Should().Be(i % 2 == 0);
        }

        _messageServiceMock.Verify(
            x => x.GetPagedAsync("chat-456", 0, 1000, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenHighPageIndex_ShouldPassCorrectParameters()
    {
        // Arrange
        var query = new GetMessagesQuery("sender-123", "chat-456", 999, 50);
        var cancellationToken = CancellationToken.None;

        var messageServiceModels = new List<MessageServiceModel>();
        var messageDtos = new List<MessageDto>();

        SetupValidationSuccess(query, cancellationToken);
        SetupMessageServiceSuccess(query, messageServiceModels, cancellationToken);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEmpty();

        _messageServiceMock.Verify(
            x => x.GetPagedAsync("chat-456", 999, 50, cancellationToken),
            Times.Once);
    }

    private void SetupValidationSuccess(GetMessagesQuery query, CancellationToken cancellationToken)
    {
        // The validator uses IChatService internally, but we don't need to mock it directly
        // since the handler creates the validator internally. The validation will succeed
        // if no exception is thrown from the validator.
        
        // We need to setup the chat service methods that the validator uses
        _chatServiceMock
            .Setup(x => x.IsChatIdValidAsync(query.ChatId, cancellationToken))
            .ReturnsAsync(true);

        _chatServiceMock
            .Setup(x => x.IsAccountChatMemberAsync(query.ChatId, query.SenderId, cancellationToken))
            .ReturnsAsync(true);
    }

    private void SetupValidationFailure(GetMessagesQuery query, CancellationToken cancellationToken)
    {
        _chatServiceMock
            .Setup(x => x.IsChatIdValidAsync(query.ChatId, cancellationToken))
            .ReturnsAsync(false);
    }

    private void SetupMessageServiceSuccess(GetMessagesQuery query, List<MessageServiceModel> messages, CancellationToken cancellationToken)
    {
        _messageServiceMock
            .Setup(x => x.GetPagedAsync(query.ChatId, query.PageIndex, query.MessagesPerPage, cancellationToken))
            .ReturnsAsync(messages);
    }

    private void SetupMapperSuccess(List<MessageServiceModel> serviceModels, List<MessageDto> dtos)
    {
        for (int i = 0; i < serviceModels.Count; i++)
        {
            _mapperMock
                .Setup(x => x.Map<MessageDto>(serviceModels[i]))
                .Returns(dtos[i]);
        }
    }

    private void VerifyAllInteractions(GetMessagesQuery query, List<MessageServiceModel> messages, CancellationToken cancellationToken)
    {
        _messageServiceMock.Verify(
            x => x.GetPagedAsync(query.ChatId, query.PageIndex, query.MessagesPerPage, cancellationToken),
            Times.Once);

        foreach (var message in messages)
        {
            _mapperMock.Verify(
                x => x.Map<MessageDto>(message),
                Times.Once);
        }
    }
}