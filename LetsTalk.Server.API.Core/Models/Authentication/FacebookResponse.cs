using Newtonsoft.Json;

namespace LetsTalk.Server.Core.Models.Authentication;

public class FacebookResponse
{
    public string? Id { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    public FacebookPicture? Picture { get; set; }
}
