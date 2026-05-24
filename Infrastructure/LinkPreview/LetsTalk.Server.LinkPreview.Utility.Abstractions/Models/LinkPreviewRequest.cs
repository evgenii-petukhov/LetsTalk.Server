namespace LetsTalk.Server.LinkPreview.Utility.Abstractions.Models
{
    public class LinkPreviewRequest
    {
        public string? Url { get; set; }

        public string? SecretKey { get; set; }

        public string? MessageId { get; set; }

        public string? ChatId { get; set; }

        public string? Token { get; set; }

        public string? ApiUrl { get; set; }
    }
}
