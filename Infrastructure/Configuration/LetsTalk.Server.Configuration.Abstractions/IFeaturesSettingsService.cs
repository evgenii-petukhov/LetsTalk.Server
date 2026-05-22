using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Configuration.Abstractions;

public interface IFeaturesSettingsService
{
    FileStorageTypes GetFileStorageType();
}
