using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Kafka.Models;

public class IncomingCallRequest
{
    public string? CallId { get; set; }

    public string? Offer { get; set; }

    public string? ChatId { get; set; }

    public AccountDto? Caller { get; set; }
}
