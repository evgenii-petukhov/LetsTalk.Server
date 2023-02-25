using Newtonsoft.Json;

namespace LetsTalk.Server.Identity.Models;

public class FacebookResponse
{
    public string? Id { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }
    
    public FacebookPicture? Picture { get; set; }
}
