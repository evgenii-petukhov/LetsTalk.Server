namespace LetsTalk.Server.Configuration.Models;

public class FileStorageSettings
{
    public string? BasePath { get; set; }
    public string? ImageFolder { get; set; }
    public int AvatarMaxWidth { get; set; }
    public int AvatarMaxHeight { get; set; }
    public int PictureMaxWidth { get; set; }
    public int PictureMaxHeight { get; set; }
    public int ImagePreviewMaxWidth { get; set; }
    public int ImagePreviewMaxHeight { get; set; }
}
