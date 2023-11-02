namespace LetsTalk.Server.Configuration.Models;

public class MessagingSettings
{
    public int MessagesPerPage { get; set; }

    public int CacheExpirationIntervalInSeconds { get; set; }
}