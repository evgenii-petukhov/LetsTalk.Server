using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class FileStorageTypeAttribute(FileStorageTypes fileStorageTypeId) : BaseEnumIdAttribute<FileStorageTypes>(fileStorageTypeId)
{
}
