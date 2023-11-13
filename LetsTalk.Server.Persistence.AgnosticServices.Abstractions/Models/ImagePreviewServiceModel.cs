namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class ImagePreviewServiceModel
{
    public int MessageId { get; set; }

    public int Id { get; set; }

    public int AccountId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}
