﻿using LetsTalk.Server.API.Models;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IFacebookService
{
    Task<LoginResponseDto> Login(LoginServiceInput model);
}