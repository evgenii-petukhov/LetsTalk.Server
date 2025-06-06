﻿namespace LetsTalk.Server.Persistence.AgnosticServices.Models;

public class ChatServiceModel
{
    public string? Id { get; set; }

    public int? AccountTypeId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? ChatName { get; set; }

    public int? UnreadCount { get; set; }

    public long? LastMessageDate { get; set; }

    public string? LastMessageId { get; set; }

    public ImageServiceModel? Image { get; set; }

    public bool IsIndividual { get; set; }

    public List<string>? AccountIds { get; set; }
}
