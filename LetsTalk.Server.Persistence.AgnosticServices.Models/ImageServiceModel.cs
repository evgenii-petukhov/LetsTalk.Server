namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class ImageServiceModel
{
    public string? Id { get; set; }

    public int ImageFormatId { get; set; }

    public int ImageRoleId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}
