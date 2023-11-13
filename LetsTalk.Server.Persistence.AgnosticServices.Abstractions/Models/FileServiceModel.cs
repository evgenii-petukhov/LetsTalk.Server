namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class FileServiceModel
{
    public int Id { get; set; }

    public string? FileName { get; protected set; }

    public int FileTypeId { get; protected set; }
}
