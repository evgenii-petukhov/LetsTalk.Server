using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.API.Models.LoginByEmail;
using LetsTalk.Server.Core.Features.Authentication.Commands.Login;
using LetsTalk.Server.Core.Features.Authentication.Commands.LoginByEmail;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<LoginByEmailRequest, LoginByEmailCommand>();
        CreateMap<LoginCommand, LoginServiceInput>();
    }
}
