using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Models;
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

    public void TrackOutgoingCallStarted(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics)
    {
        _telemetryClient.TrackEvent(VideoCallEventName, new Dictionary<string, string>
        {
            [nameof(callId)] = callId,
            [nameof(chatId)] = chatId,
            [nameof(accountId)] = accountId,
            ["role"] = "caller",
            ["event"] = "offer created",
            [nameof(iceGatheringMs)] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            [nameof(iceCollectedAll)] = iceCollectedAll.ToString(),
            ["connectionState"] = connectionDiagnostics.ConnectionState!,
            ["localCandidateTypes"] = connectionDiagnostics.LocalCandidateTypes!,
            ["remoteCandidateTypes"] = connectionDiagnostics.RemoteCandidateTypes!,
            ["browser"] = connectionDiagnostics.Browser!,
            ["platform"] = connectionDiagnostics.Platform!
        });
    }

    public void TrackIncomingCallHandled(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics)
    {
        _telemetryClient.TrackEvent(VideoCallEventName, new Dictionary<string, string>
        {
            [nameof(callId)] = callId,
            [nameof(chatId)] = chatId,
            [nameof(accountId)] = accountId,
            ["role"] = "callee",
            ["event"] = "answer created",
            [nameof(iceGatheringMs)] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            [nameof(iceCollectedAll)] = iceCollectedAll.ToString(),
            ["connectionState"] = connectionDiagnostics.ConnectionState!,
            ["localCandidateTypes"] = connectionDiagnostics.LocalCandidateTypes!,
            ["remoteCandidateTypes"] = connectionDiagnostics.RemoteCandidateTypes!,
            ["browser"] = connectionDiagnostics.Browser!,
            ["platform"] = connectionDiagnostics.Platform!,
        });
    }
}
