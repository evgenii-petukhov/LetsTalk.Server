using Newtonsoft.Json;

namespace LetsTalk.Server.API.Core.Models.Authentication;

public class VkError
{
    [JsonProperty("error_code")]
    public int Code { get; set; }

    [JsonProperty("error_msg")]
    public string? Message { get; set; }
}
