using LetsTalk.Server.API.Core.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

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
        string errorMessage,
        string stackTrace);
}
