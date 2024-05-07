using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Models.Message;

public class CreateMessageResponse
{
    public MessageDto? Dto { get; set; }

    public string? Url { get; set; }
}
