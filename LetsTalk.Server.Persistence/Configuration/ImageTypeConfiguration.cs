using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageTypeConfiguration : IEntityTypeConfiguration<ImageType>
{
    public void Configure(EntityTypeBuilder<ImageType> builder)
    {
        builder.HasData(
            Enum.GetValues<ImageTypes>().Select(x => new ImageType
            {
                Id = (int)x,
                Name = Enum.GetName(x)
            }));
    }
}
