using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class ProfileMongoDBService(
    IAccountRepository accountRepository,
    IMapper mapper) : IProfileAgnosticService
{
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ProfileServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.UpdateProfileAsync(accountId, firstName, lastName, cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.UpdateProfileAsync(
            accountId,
            firstName,
            lastName,
            imageId,
            width,
            height,
            imageFormat,
            fileStorageType,
            cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }
}
