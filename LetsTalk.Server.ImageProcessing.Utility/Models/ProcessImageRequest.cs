namespace LetsTalk.Server.ImageProcessing.Utility.Models;

public class ProcessImageRequest
{
    public string? FileName { get; set; }

    public string? BucketName { get; set; }

    public int MaxWidth { get; set; }

    public int MaxHeight { get; set; }

    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public int FileStorageTypeId { get; set; }

    public string? Token { get; set; }

    public string? ApiUrl { get; set; }
}
