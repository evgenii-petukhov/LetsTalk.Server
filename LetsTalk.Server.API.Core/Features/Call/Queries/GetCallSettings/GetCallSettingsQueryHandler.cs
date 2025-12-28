using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Features.Call.Queries.GetCallSettings;

public class GetCallSettingsQueryHandler(
    IIceServerConfigurationService iceServerConfigurationService,
    IOptions<RtcSettings> options) : IRequestHandler<GetCallSettingsQuery, CallSettingsDto>
{
    private readonly IIceServerConfigurationService _iceServerConfigurationService = iceServerConfigurationService;
    private readonly RtcSettings _rtcSettings = options.Value;

    public async Task<CallSettingsDto> Handle(GetCallSettingsQuery _, CancellationToken cancellationToken)
    {
        var iceServerConfiguration = await _iceServerConfigurationService.GetIceServerConfigurationAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(iceServerConfiguration))
        {
            throw new BadRequestException("Invalid request");
        }

        return new CallSettingsDto
        {
            IceServerConfiguration = iceServerConfiguration,
            MaxVideoDurationInSeconds = _rtcSettings.MaxVideoDurationInSeconds
        };
    }
}
