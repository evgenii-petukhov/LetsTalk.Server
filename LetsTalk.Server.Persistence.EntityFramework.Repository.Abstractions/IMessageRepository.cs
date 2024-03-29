﻿using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message>> GetPagedAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(int senderId, int recipientId, int messageId);
}
