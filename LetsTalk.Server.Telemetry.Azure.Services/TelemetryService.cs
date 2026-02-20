using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Telemetry.Abstractions;
using LetsTalk.Server.Telemetry.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace LetsTalk.Server.API.Core.Services;

public class TelemetryService(
    IOptions<ApplicationInsightsSettings> options) : ITelemetryService
{
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
    private const string Error = "error";
    private const string ErrorType = "errorType";
    private const string StackTrace = "stackTrace";

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
            [Role] = RtcRoles.Caller.ToString(),
            [Event] = RtcEvents.StartOutgoingCall.ToString(),
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
            [Role] = RtcRoles.Callee.ToString(),
            [Event] = RtcEvents.HandleIncomingCall.ToString(),
            [IceGatheringMs] = iceGatheringMs.ToString(CultureInfo.InvariantCulture),
            [IceCollectedAll] = iceCollectedAll.ToString(),
            [ConnectionState] = connectionDiagnostics.ConnectionState!,
            [LocalCandidateTypes] = connectionDiagnostics.LocalCandidateTypes!,
            [RemoteCandidateTypes] = connectionDiagnostics.RemoteCandidateTypes!,
            [Browser] = connectionDiagnostics.Browser!,
            [Platform] = connectionDiagnostics.Platform!,
        });
    }

    public void TrackConnectionEstablished(
        string callId,
        string chatId,
        string accountId,
        ConnectionDiagnostics connectionDiagnostics)
    {
        _telemetryClient.TrackEvent(VideoCall, new Dictionary<string, string>
        {
            [CallId] = callId,
            [ChatId] = chatId,
            [AccountId] = accountId,
            [Event] = RtcEvents.ConnectionEstablished.ToString(),
            [ConnectionState] = connectionDiagnostics.ConnectionState!,
            [LocalCandidateTypes] = connectionDiagnostics.LocalCandidateTypes!,
            [RemoteCandidateTypes] = connectionDiagnostics.RemoteCandidateTypes!,
            [Browser] = connectionDiagnostics.Browser!,
            [Platform] = connectionDiagnostics.Platform!,
        });
    }

    public void TrackConnectionFailed(
        string callId,
        string chatId,
        string accountId,
        ConnectionDiagnostics connectionDiagnostics,
        RtcErrorType errorType,
        string errorMessage,
        string stackTrace)
    {
        _telemetryClient.TrackEvent(VideoCall, new Dictionary<string, string>
        {
            [CallId] = callId,
            [ChatId] = chatId,
            [AccountId] = accountId,
            [Event] = RtcEvents.Error.ToString(),
            [ConnectionState] = connectionDiagnostics.ConnectionState!,
            [LocalCandidateTypes] = connectionDiagnostics.LocalCandidateTypes!,
            [RemoteCandidateTypes] = connectionDiagnostics.RemoteCandidateTypes!,
            [Browser] = connectionDiagnostics.Browser!,
            [Platform] = connectionDiagnostics.Platform!,
            [Error] = errorMessage!,
            [ErrorType] = errorType.ToString(),
            [StackTrace] = stackTrace,
        });
    }
}
