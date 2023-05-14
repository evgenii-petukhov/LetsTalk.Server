using LetsTalk.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageTypeConfiguration : IEntityTypeConfiguration<ImageType>
{
    public void Configure(EntityTypeBuilder<ImageType> builder)
    {
        builder.HasData(
            new ImageType
            {
                Id = 1,
                Name = "Avatar"
            },
            new ImageType
            {
                Id = 2,
                Name = "Message"
            });
    }
}
