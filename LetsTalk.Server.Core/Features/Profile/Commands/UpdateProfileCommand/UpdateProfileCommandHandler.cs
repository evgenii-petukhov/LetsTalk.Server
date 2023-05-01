using LetsTalk.Server.API.Models.UpdateProfile;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResponse>
{
    Task<UpdateProfileResponse> IRequestHandler<UpdateProfileCommand, UpdateProfileResponse>.Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
