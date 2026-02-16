using System.Text.Json.Serialization;

namespace LetsTalk.Server.API.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RtcRoles
{
    NotSet = 0,
    Caller = 1,
    Callee = 2
}
