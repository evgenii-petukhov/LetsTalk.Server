namespace LetsTalk.Server.Identity.Models;

public class VkResponse
{
    public VkError Error { get; set; }

    public VkInnerResponse[]? Response { get; set; }
}