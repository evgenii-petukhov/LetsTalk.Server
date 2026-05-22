using LetsTalk.Server.Configuration.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Configuration.Services;

public class FeaturesSettingsService(IOptions<FeaturesSettings> options) : IFeaturesSettingsService
{
    private readonly FeaturesSettings _featuresSettings = options.Value;

    public FileStorageTypes GetFileStorageType()
    {
        return Enum.TryParse<FileStorageTypes>(_featuresSettings.FileStorage, out var fileStorageType)
            ? fileStorageType
            : FileStorageTypes.Local;
    }
}
