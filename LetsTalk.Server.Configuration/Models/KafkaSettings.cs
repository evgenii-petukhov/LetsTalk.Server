﻿namespace LetsTalk.Server.Configuration.Models;

public class KafkaSettings
{
    public string? Url { get; set; }

    public KafkaEventSettings? MessageNotification { get; set; }

    public KafkaEventSettings? LinkPreviewRequest { get; set; }

    public KafkaEventSettings? LinkPreviewNotification { get; set; }

    public KafkaEventSettings? ImageResizeRequest { get; set; }

    public KafkaEventSettings? SetImageDimensionsRequest { get; set; }

    public KafkaEventSettings? ImagePreviewNotification { get; set; }

    public KafkaEventSettings? RemoveImageRequest { get; set; }

    public KafkaEventSettings? SendLoginCodeRequest { get; set; }
}
