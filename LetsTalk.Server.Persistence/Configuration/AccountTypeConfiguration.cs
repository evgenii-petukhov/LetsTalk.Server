using LetsTalk.Server.Domain;
using LetsTalk.Server.Models.Account.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LetsTalk.Server.Persistence.Configuration;

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.HasData(
            Enum.GetValues<AccountTypes>().Select(x => new AccountType
            {
                Id = (int)x,
                Name = Enum.GetName(x)
            }));
    }
}
