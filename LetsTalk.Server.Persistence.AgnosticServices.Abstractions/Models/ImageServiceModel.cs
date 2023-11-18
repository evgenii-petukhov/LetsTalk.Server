namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class ImageServiceModel
{
    public string? Id { get; set; }

    public int ImageFormatId { get; protected set; }

    public int ImageRoleId { get; protected set; }

    public FileServiceModel? File { get; protected set; }

    public int? Width { get; protected set; }

    public int? Height { get; protected set; }
}
