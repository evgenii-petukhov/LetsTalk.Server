namespace LetsTalk.Server.API.Core.Abstractions;

public interface IIceServerConfigurationService
{
    Task<string> GetIceServerConfigurationAsync(CancellationToken cancellationToken);
}
