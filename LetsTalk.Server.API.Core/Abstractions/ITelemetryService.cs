namespace LetsTalk.Server.API.Core.Abstractions;

public interface ITelemetryService
{
    void TrackOutgoingCallStarted(
        string callId,
        string chatId,
        string accountId,
        string connectionState,
        string localCandidateTypes,
        string remoteCandidateTypes,
        string browser,
        string platform,
        int iceGatheringMs,
        bool iceCollectedAll);

    void TrackIncomingCallHandled(
        string callId,
        string chatId,
        string accountId,
        string connectionState,
        string localCandidateTypes,
        string remoteCandidateTypes,
        string browser,
        string platform,
        int iceGatheringMs,
        bool iceCollectedAll);
}
