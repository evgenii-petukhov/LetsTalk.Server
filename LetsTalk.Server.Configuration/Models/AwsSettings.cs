namespace LetsTalk.Server.Configuration.Models;

public class AwsSettings
{
    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }

    public string? Region { get; set; }

    public string? BucketName { get; set; }
}
