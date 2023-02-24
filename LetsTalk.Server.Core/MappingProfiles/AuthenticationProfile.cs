using AutoMapper;
using LetsTalk.Server.Core.Features.Authentication.Commands;
using LetsTalk.Server.Core.Models.Authentication;

namespace LetsTalk.Server.Core.MappingProfiles;

public class AuthenticationProfile : Profile
{
	public AuthenticationProfile()
	{
		CreateMap<LoginRequest, LoginCommand>();
	}
}
