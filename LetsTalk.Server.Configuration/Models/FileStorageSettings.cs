namespace LetsTalk.Server.Configuration.Models;

public class FileStorageSettings
{
    public string? BasePath { get; set; }
    public string? ImageFolder { get; set; }
    public int AvatarMaxWidth { get; set; }
    public int AvatarMaxHeight { get; set; }
}
