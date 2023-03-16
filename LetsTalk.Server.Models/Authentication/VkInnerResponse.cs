using Newtonsoft.Json;

namespace LetsTalk.Server.Models.Authentication;

public class VkInnerResponse
{
    public string? Id { get; set; }

    [JsonProperty("photo_max")]
    public string? PictureUrl { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }
}