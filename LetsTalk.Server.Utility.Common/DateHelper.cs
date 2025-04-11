namespace LetsTalk.Server.Utility.Common;

public static class DateHelper
{
    private static readonly DateTime Origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public static long GetUnixTimestamp()
    {
        var diff = DateTime.UtcNow - Origin;
        return (long)diff.TotalSeconds;
    }
}
