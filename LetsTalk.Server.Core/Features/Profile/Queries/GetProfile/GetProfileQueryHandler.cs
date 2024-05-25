using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Queries.GetProfile;

public class GetProfileQueryHandler(
    IMapper mapper,
    IProfileService profileService) : IRequestHandler<GetProfileQuery, ProfileDto>
{
    private readonly IMapper _mapper = mapper;
    private readonly IProfileService _profileService = profileService;

    public async Task<ProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _profileService.GetProfileAsync(request.Id, cancellationToken);
        return _mapper.Map<ProfileDto>(accounts);
    }
}
