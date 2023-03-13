namespace LetsTalk.Server.Core.Helpers;

public static class DateTimeHelper
{
    public static long ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)diff.TotalSeconds;
    }
}
