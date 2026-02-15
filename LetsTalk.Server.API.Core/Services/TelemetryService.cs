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
    private static class Roles
    {
        public const string Caller = "caller";
        public const string Callee = "callee";
    }

    private static class Events
    {
        public const string StartOutgoingCall = "StartOutgoingCall";
        public const string HandleIncomingCall = "HandleIncomingCall";
    }

    private const string VideoCall = "video-call";
    private const string CallId = "callId";
    private const string ChatId = "chatId";
    private const string AccountId = "accountId";
    private const string Role = "role";
    private const string Event = "event";
    private const string IceGatheringMs = "iceGatheringMs";
    private const string IceCollectedAll = "iceCollectedAll";
    private const string ConnectionState = "connectionState";
    private const string LocalCandidateTypes = "localCandidateTypes";
    private const string RemoteCandidateTypes = "remoteCandidateTypes";
    private const string Browser = "browser";
    private const string Platform = "platform";

    private readonly TelemetryClient _telemetryClient = new(new TelemetryConfiguration
    {
        ConnectionString = options.Value.ConnectionString,
    });

    public void TrackStartOutgoingCall(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics)
    {
        _telemetryClient.TrackEvent(VideoCall, new Dictionary<string, string>
        {
            [CallId] = callId,
            [ChatId] = chatId,
            [AccountId] = accountId,
            [Role] = Roles.Caller,
            [Event] = Events.StartOutgoingCall,
            [IceGatheringMs] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            [IceCollectedAll] = iceCollectedAll.ToString(),
            [ConnectionState] = connectionDiagnostics.ConnectionState!,
            [LocalCandidateTypes] = connectionDiagnostics.LocalCandidateTypes!,
            [RemoteCandidateTypes] = connectionDiagnostics.RemoteCandidateTypes!,
            [Browser] = connectionDiagnostics.Browser!,
            [Platform] = connectionDiagnostics.Platform!
        });
    }

    public void TrackHandleIncomingCall(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics)
    {
        _telemetryClient.TrackEvent(VideoCall, new Dictionary<string, string>
        {
            [CallId] = callId,
            [ChatId] = chatId,
            [AccountId] = accountId,
            [Role] = Roles.Callee,
            [Event] = Events.HandleIncomingCall,
            [IceGatheringMs] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            [IceCollectedAll] = iceCollectedAll.ToString(),
            [ConnectionState] = connectionDiagnostics.ConnectionState!,
            [LocalCandidateTypes] = connectionDiagnostics.LocalCandidateTypes!,
            [RemoteCandidateTypes] = connectionDiagnostics.RemoteCandidateTypes!,
            [Browser] = connectionDiagnostics.Browser!,
            [Platform] = connectionDiagnostics.Platform!,
        });
    }
}
