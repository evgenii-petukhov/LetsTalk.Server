namespace LetsTalk.Server.Models.Authentication;

public class VkResponse
{
    public VkError? Error { get; set; }

    public VkInnerResponse[]? Response { get; set; }
}