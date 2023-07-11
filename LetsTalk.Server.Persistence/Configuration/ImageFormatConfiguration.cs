using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageFormatConfiguration : IEntityTypeConfiguration<ImageFormat>
{
    public void Configure(EntityTypeBuilder<ImageFormat> builder)
    {
        builder.HasData(
            Enum.GetValues<ImageFormats>().Select(x => new ImageFormat
            {
                Id = (int)x,
                Name = Enum.GetName(x)
            }));
    }
}
