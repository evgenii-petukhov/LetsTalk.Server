using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommand: IRequest
{
    public int? AccountId { get; set; }

    public string? Email { get; set; }

    public int? ImageId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}