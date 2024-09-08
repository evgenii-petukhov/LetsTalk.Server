using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<EmailLoginRequest, EmailLoginCommand>();
    }
}
