using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class FileTypeConfiguration : IEntityTypeConfiguration<FileType>
{
    public void Configure(EntityTypeBuilder<FileType> builder)
    {
        builder.HasData(
            Enum.GetValues<FileTypes>().Select(x => new FileType((int)x, Enum.GetName(x)!)));
    }
}
