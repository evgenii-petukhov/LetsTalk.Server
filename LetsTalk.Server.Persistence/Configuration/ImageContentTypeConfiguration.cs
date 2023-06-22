using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageContentTypeConfiguration : IEntityTypeConfiguration<ImageContentType>
{
    public void Configure(EntityTypeBuilder<ImageContentType> builder)
    {
        builder.HasData(
            Enum.GetValues<ImageContentTypes>().Select(x => new ImageContentType
            {
                Id = (int)x,
                Name = Enum.GetName(x)
            }));
    }
}
