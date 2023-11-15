namespace LetsTalk.Server.Persistence.Utility;

public static class DateHelper
{
    public static long GetUnixTimestamp()
    {
        DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = DateTime.UtcNow - origin;
        return (long)diff.TotalSeconds;
    }
}
