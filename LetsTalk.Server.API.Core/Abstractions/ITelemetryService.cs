using LetsTalk.Server.API.Core.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface ITelemetryService
{
    void TrackOutgoingCallStarted(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics);

    void TrackIncomingCallHandled(
        string callId,
        string chatId,
        string accountId,
        int iceGatheringMs,
        bool iceCollectedAll,
        ConnectionDiagnostics connectionDiagnostics);
}
