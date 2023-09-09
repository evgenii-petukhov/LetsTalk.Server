using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class FileConfiguration : IEntityTypeConfiguration<Domain.File>
{
    public void Configure(EntityTypeBuilder<Domain.File> builder)
    {
        builder
            .HasMany(e => e.Images)
            .WithOne(e => e.File)
            .HasForeignKey(e => e.FileId);
    }
}
