namespace LetsTalk.Server.Models.Kafka;

public class EstablishConnectionRequest
{
    public string? CallId { get; set; }

    public string? Answer { get; set; }

    public string? ChatId { get; set; }
}
