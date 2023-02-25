using AutoMapper;
using LetsTalk.Server.API.Models.Authentication;
using LetsTalk.Server.Core.Features.Authentication.Commands;

namespace LetsTalk.Server.API.MappingProfiles;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<LoginRequest, LoginCommand>();
    }
}
