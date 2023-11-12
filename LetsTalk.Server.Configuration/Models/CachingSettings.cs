namespace LetsTalk.Server.Configuration.Models;

public class CachingSettings
{
    public int MessagesCacheLifeTimeInSeconds { get; set; }

    public int ContactsCacheLifeTimeInSeconds { get; set; }

    public int ProfileCacheLifeTimeInSeconds { get; set; }

    public int ImagesCacheLifeTimeInSeconds { get; set; }

    public int ImageSizeThresholdInBytes { get; set; }
}