using LetsTalk.Server.Telemetry.Models;

namespace LetsTalk.Server.Telemetry.Abstractions;

public interface ITelemetryService
{
    void TrackStartOutgoingCall(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics);

    void TrackHandleIncomingCall(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics);

    void TrackConnectionEstablished(
        string callId,
        string chatId,
        string accountId,
        ConnectionDiagnostics connectionDiagnostics);

    void TrackConnectionFailed(
        string callId,
        string chatId,
        string accountId,
        ConnectionDiagnostics connectionDiagnostics,
        RtcErrorType errorType,
        string errorMessage,
        string stackTrace);
}
