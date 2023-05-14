using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageFileTypeConfiguration : IEntityTypeConfiguration<ImageFileType>
{
    public void Configure(EntityTypeBuilder<ImageFileType> builder)
    {
        builder.HasData(
            Enum.GetValues<ImageFileTypes>().Select(x => new ImageFileType
            {
                Id = (int)x,
                Name = Enum.GetName(x)
            }));
    }
}
