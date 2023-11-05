using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Commands.UpdateProfileCommand;

public class UpdateProfileCommand : IRequest<AccountDto>
{
    public int? AccountId { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? ImageId { get; set; }
}