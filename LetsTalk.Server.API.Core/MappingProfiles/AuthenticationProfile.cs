using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.API.Core.Commands;

namespace LetsTalk.Server.API.Core.MappingProfiles;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<EmailLoginRequest, EmailLoginCommand>();
    }
}
