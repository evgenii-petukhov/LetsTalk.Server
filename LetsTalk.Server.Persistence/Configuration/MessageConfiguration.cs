using LetsTalk.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder
            .HasOne(e => e.Image)
            .WithOne()
            .HasForeignKey<Message>(x => x.ImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(e => e.ImagePreview)
            .WithOne()
            .HasForeignKey<Message>(x => x.ImagePreviewId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(e => e.LinkPreview)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
