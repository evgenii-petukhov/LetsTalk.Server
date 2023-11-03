namespace LetsTalk.Server.Configuration.Models;

public class CachingSettings
{
    public int MessagesCacheLifeTimeInSeconds { get; set; }

    public int ContactsCacheLifeTimeInSeconds { get; set; }
}