namespace LetsTalk.Server.Utility.Common;

public static class DateHelper
{
    private static DateTime Origin = new(1970, 1, 1, 0, 0, 0, 0);

    public static long GetUnixTimestamp()
    {
        var diff = DateTime.UtcNow - Origin;
        return (long)diff.TotalSeconds;
    }
}
