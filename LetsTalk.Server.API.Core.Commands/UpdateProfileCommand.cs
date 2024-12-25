using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Commands;

public class UpdateProfileCommand : IRequest<ProfileDto>
{
    public string? AccountId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public ImageRequestModel? Image { get; set; }
}