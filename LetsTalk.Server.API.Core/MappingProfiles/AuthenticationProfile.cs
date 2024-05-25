using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Core.Features.Authentication.Commands.Login;
using LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<EmailLoginRequest, EmailLoginCommand>();        
    }
}
