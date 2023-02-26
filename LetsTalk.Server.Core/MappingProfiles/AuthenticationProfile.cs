using AutoMapper;
using LetsTalk.Server.Core.Features.Authentication.Commands;
using LetsTalk.Server.Models.Authentication;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<LoginCommand, LoginServiceInput>();
    }
}
