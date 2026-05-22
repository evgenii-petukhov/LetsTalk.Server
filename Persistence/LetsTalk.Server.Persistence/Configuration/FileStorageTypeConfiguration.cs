using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class FileStorageTypeConfiguration : IEntityTypeConfiguration<FileStorageType>
{
    public void Configure(EntityTypeBuilder<FileStorageType> builder)
    {
        builder.HasData(
            Enum.GetValues<FileStorageTypes>().Select(x => new FileStorageType((int)x, Enum.GetName(x)!)));
    }
}
