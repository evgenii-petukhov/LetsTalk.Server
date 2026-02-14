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

    public void TrackOutgoingCallStarted(
        string callId,
        string chatId,
        string accountId,
        string connectionState,
        string localCandidateTypes,
        string remoteCandidateTypes,
        string browser,
        string platform,
        int iceGatheringMs,
        bool iceCollectedAll)
    {
        _telemetryClient.TrackEvent(VideoCallEventName, new Dictionary<string, string>
        {
            [nameof(callId)] = callId,
            [nameof(chatId)] = chatId,
            [nameof(accountId)] = accountId,
            ["role"] = "caller",
            ["event"] = "offer created",
            [nameof(connectionState)] = connectionState,
            [nameof(localCandidateTypes)] = localCandidateTypes,
            [nameof(remoteCandidateTypes)] = remoteCandidateTypes,
            [nameof(browser)] = browser,
            [nameof(platform)] = platform,
            [nameof(iceGatheringMs)] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            [nameof(iceCollectedAll)] = iceCollectedAll.ToString()
        });
    }

    public void TrackIncomingCallHandled(
        string callId,
        string chatId,
        string accountId,
        string connectionState,
        string localCandidateTypes,
        string remoteCandidateTypes,
        string browser,
        string platform,
        int iceGatheringMs,
        bool iceCollectedAll)
    {
        _telemetryClient.TrackEvent(VideoCallEventName, new Dictionary<string, string>
        {
            [nameof(callId)] = callId,
            [nameof(chatId)] = chatId,
            [nameof(accountId)] = accountId,
            ["role"] = "callee",
            ["event"] = "answer created",
            [nameof(connectionState)] = connectionState,
            [nameof(localCandidateTypes)] = localCandidateTypes,
            [nameof(remoteCandidateTypes)] = remoteCandidateTypes,
            [nameof(browser)] = browser,
            [nameof(platform)] = platform,
            [nameof(iceGatheringMs)] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            [nameof(iceCollectedAll)] = iceCollectedAll.ToString()
        });
    }
}
