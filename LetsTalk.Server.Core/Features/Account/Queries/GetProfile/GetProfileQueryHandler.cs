﻿using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler(
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<AccountDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetByIdAsync(request.Id, cancellationToken);
        return _mapper.Map<AccountDto>(accounts);
    }
}