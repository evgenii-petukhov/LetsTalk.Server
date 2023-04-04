using Microsoft.Extensions.Configuration;

namespace LetsTalk.Server.Configuration.Models;

public class KafkaEventSettings
{
    public string? Topic { get; set; }
    public string? Producer { get; set; }
    public string? GroupId { get; set; }
}
