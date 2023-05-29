using LetsTalk.Server.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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
