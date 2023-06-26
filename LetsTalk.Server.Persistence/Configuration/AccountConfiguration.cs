using LetsTalk.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class ImageConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder
            .HasOne(e => e.Image)
            .WithOne(e => e.Account)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
