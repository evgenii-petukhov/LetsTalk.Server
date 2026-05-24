namespace LetsTalk.Server.Models.Dtos;

public class CallSettingsDto
{
    public string? IceServerConfiguration { get; set; }

    public int MaxVideoDurationInSeconds { get; set; }
}
