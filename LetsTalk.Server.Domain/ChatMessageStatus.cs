﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetsTalk.Server.Domain;

[Table("chatmessagestatuses")]
[PrimaryKey("ChatMemberId", "MessageId")]
public class ChatMessageStatus
{
    public ChatMember? ChatMember { get; protected set; }

    public int ChatMemberId { get; protected set; }

    public Message? Message { get; protected set; }

    public int MessageId { get; protected set; }

    public bool IsRead { get; protected set; }

    public long? DateReadUnix { get; protected set; }

    protected ChatMessageStatus()
    {
    }
}