using System.Text.Json.Serialization;

namespace LetsTalk.Server.Telemetry.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RtcErrorType
{
    NotSet = 0,
    Connection = 1,
    IceServer = 2,
    Media = 3,
}
