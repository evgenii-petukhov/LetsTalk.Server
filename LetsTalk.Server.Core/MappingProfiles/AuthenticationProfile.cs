using AutoMapper;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.Core.Features.Authentication.Commands;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<LoginCommand, LoginServiceInput>();
    }
}
