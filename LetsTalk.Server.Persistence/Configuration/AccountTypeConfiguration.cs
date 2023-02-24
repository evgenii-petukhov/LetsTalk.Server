using LetsTalk.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.HasData(
            new AccountType
            {
                Id = 1,
                Name = "Facebook"
            },
            new AccountType
            {
                Id = 2,
                Name = "VK"
            }
        );
    }
}
