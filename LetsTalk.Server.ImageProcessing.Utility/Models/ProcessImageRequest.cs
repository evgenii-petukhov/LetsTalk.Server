namespace LetsTalk.Server.ImageProcessing.Utility.Models;

public class ProcessImageRequest
{
    public string? FileName { get; set; }

    public string? BucketName { get; set; }

    public int MaxWidth { get; set; }

    public int MaxHeight { get; set; }
}
