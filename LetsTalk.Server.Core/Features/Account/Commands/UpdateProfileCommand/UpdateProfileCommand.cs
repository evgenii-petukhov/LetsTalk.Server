using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Commands.UpdateProfileCommand;

public class UpdateProfileCommand : IRequest<AccountDto>
{
    public string? AccountId { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public ImageRequestModel? Image { get; set; }
}