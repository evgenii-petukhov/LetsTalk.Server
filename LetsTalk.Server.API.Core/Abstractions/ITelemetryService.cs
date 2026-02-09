namespace LetsTalk.Server.API.Core.Abstractions;

public interface ITelemetryService
{
    void TrackOutgoingCallStarted(string callId, string chatId, string accountId, int iceGatheringMs, bool collectedAll);

    void TrackIncomingCallHandled(string callId, string chatId, string accountId, int iceGatheringMs, bool collectedAll);
}
