namespace LetsTalk.Server.Core.Models;

public class VkResponse
{
    public VkError? Error { get; set; }

    public VkInnerResponse[]? Response { get; set; }
}