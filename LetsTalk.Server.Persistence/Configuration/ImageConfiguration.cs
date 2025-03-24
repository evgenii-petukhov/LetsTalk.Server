using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder
            .Property(e => e.FileStorageTypeId)
            .HasDefaultValue((int)FileStorageTypes.Local);
    }
}
