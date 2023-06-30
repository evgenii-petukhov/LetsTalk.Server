using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, AccountDto>
{
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler(
        IAccountDataLayerService accountDataLayerService,
        IMapper mapper)
    {
        _accountDataLayerService = accountDataLayerService;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _accountDataLayerService.GetByIdOrDefaultAsync(request.Id, x => x, cancellationToken: cancellationToken);
        return _mapper.Map<AccountDto>(accounts);
    }
}
