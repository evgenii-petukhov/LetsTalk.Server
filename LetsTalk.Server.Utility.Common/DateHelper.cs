namespace LetsTalk.Server.Utility.Common;

public static class DateHelper
{
    private static readonly DateTime Origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public static long GetUnixTimestamp()
    {
        return GetUnixTimestamp(DateTime.UtcNow);
    }

    public static long GetUnixTimestamp(DateTime dateUtc)
    {
        var diff = dateUtc - Origin;
        return (long)diff.TotalSeconds;
    }
}
