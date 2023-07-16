using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageRoleConfiguration : IEntityTypeConfiguration<ImageRole>
{
    public void Configure(EntityTypeBuilder<ImageRole> builder)
    {
        builder.HasData(
            Enum.GetValues<ImageRoles>().Select(x => new ImageRole
            {
                Id = (int)x,
                Name = Enum.GetName(x)
            }));
    }
}
