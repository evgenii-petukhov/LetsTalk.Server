using System.Text.Json.Serialization;

namespace LetsTalk.Server.API.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RtcEvents
{
    NotSet = 0,
    StartOutgoingCall = 1,
    HandleIncomingCall = 2,
    ConnectionEstablished = 3,
    Error = 4,
}
