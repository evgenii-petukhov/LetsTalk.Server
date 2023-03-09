using Newtonsoft.Json;

namespace LetsTalk.Server.Identity.Models;

public class VkError
{
    [JsonProperty("error_code")]
    public int Code { get; set; }

    [JsonProperty("error_message")]
    public string? Message { get; set; }
}
