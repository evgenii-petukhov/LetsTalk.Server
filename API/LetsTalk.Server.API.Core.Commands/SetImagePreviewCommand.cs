using LetsTalk.Server.Persistence.Enums;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public class SetImagePreviewCommand : IRequest<Unit>
{
    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public string? Filename { get; set; }

    public ImageFormats ImageFormat { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public FileStorageTypes FileStorageTypeId { get; set; }
}
