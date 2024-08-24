namespace LetsTalk.Server.Kafka.Models;

public class SendLoginCodeRequest
{
    public string? Email { get; set; }

    public int Code { get; set; }
}
