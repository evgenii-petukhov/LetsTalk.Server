using LetsTalk.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageFileTypeConfiguration : IEntityTypeConfiguration<ImageFileType>
{
    public void Configure(EntityTypeBuilder<ImageFileType> builder)
    {
        builder.HasData(
            new ImageFileType
            {
                Id = 0,
                Name = "Unknown"
            },
            new ImageFileType
            {
                Id = 1,
                Name = "Jpeg"
            },
            new ImageFileType
            {
                Id = 2,
                Name = "Png"
            },
            new ImageFileType
            {
                Id = 3,
                Name = "Gif"
            });
    }
}
