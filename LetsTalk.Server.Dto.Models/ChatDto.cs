﻿namespace LetsTalk.Server.Dto.Models;

public class ChatDto : ChatDtoBase
{
    public int AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? ChatName { get; set; }

    public int UnreadCount { get; set; }

    public long LastMessageDate { get; set; }

    public string? LastMessageId { get; set; }

    public ImageDto? Image { get; set; }

    public bool IsIndividual { get; set; }

    public List<string>? AccountIds { get; set; }
}
