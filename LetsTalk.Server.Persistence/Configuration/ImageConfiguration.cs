using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageConfiguration : IEntityTypeConfiguration<Domain.Image>
{
    public void Configure(EntityTypeBuilder<Domain.Image> builder)
    {
        builder
            .HasOne(e => e.File)
            .WithOne(e => e.Image)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
