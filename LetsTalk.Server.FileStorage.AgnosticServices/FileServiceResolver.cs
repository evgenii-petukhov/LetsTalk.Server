using LetsTalk.Server.Configuration.Abstractions;
using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Abstractions.Attributes;
using LetsTalk.Server.Persistence.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LetsTalk.Server.FileStorage.AgnosticServices;

public class FileServiceResolver(
    IEnumerable<IFileService> fileServices,
    IFeaturesSettingsService featuresSettingsService) : IFileServiceResolver
{
    private readonly Dictionary<FileStorageTypes, IFileService> _fileServiceMapping = GetOpenAuthProviderMapping(fileServices);
    private readonly IFeaturesSettingsService _featuresSettingsService = featuresSettingsService;

    public IFileService Resolve()
    {
        return Resolve(_featuresSettingsService.GetFileStorageType())!;
    }

    public IFileService Resolve(FileStorageTypes fileStorageType)
    {
        return _fileServiceMapping.GetValueOrDefault(fileStorageType)!;
    }

    private static Dictionary<FileStorageTypes, IFileService> GetOpenAuthProviderMapping(IEnumerable<IFileService> fileServices)
    {
        return fileServices
            .Select(provider => new
            {
                Attribute = provider.GetType().GetCustomAttribute<FileStorageTypeAttribute>(),
                Provider = provider
            })
            .Where(t => t.Attribute != null)
            .ToDictionary(t => t.Attribute!.Id, t => t.Provider);
    }
}
