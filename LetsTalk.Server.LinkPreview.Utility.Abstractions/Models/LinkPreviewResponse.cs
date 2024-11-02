namespace LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;

public class LinkPreviewResponse
{
    public OpenGraphModel? OpenGraphModel { get; set; }

    public Exception? Exception { get; set; }
}
