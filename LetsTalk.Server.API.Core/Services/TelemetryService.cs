using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace LetsTalk.Server.API.Core.Services;

public class TelemetryService(
    IOptions<ApplicationInsightsSettings> options) : ITelemetryService
{
    private const string VideoCallEventName = "video-call";

    private readonly TelemetryClient _telemetryClient = new(new TelemetryConfiguration
    {
        ConnectionString = options.Value.ConnectionString,
    });

    public void TrackOutgoingCallStarted(string callId, string chatId, string accountId, int iceGatheringMs, bool collectedAll)
    {
        _telemetryClient.TrackEvent(VideoCallEventName, new Dictionary<string, string>
        {
            ["callId"] = callId,
            ["chatId"] = chatId,
            ["accountId"] = accountId,
            ["role"] = "caller",
            ["event"] = "offer created",
            ["iceGatheringMs"] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            ["collectedAll"] = collectedAll.ToString()
        });
    }

    public void TrackIncomingCallHandled(string callId, string chatId, string accountId, int iceGatheringMs, bool collectedAll)
    {
        _telemetryClient.TrackEvent(VideoCallEventName, new Dictionary<string, string>
        {
            ["callId"] = callId,
            ["chatId"] = chatId,
            ["accountId"] = accountId,
            ["role"] = "callee",
            ["event"] = "answer created",
            ["iceGatheringMs"] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            ["collectedAll"] = collectedAll.ToString()
        });
    }
}
