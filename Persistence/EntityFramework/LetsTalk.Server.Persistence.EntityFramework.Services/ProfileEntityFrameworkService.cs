using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using System.Globalization;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class ProfileEntityFrameworkService(
    IAccountRepository accountRepository,
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEntityFactory entityFactory) : IProfileAgnosticService
{
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IImageRepository _imageRepository = imageRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IEntityFactory _entityFactory = entityFactory;

    public async Task<ProfileServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(int.Parse(id, CultureInfo.InvariantCulture), cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsTrackingAsync(int.Parse(accountId, CultureInfo.InvariantCulture), cancellationToken);
        account.UpdateProfile(firstName, lastName);

        await _unitOfWork.SaveAsync(cancellationToken);

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
        var image = _entityFactory.CreateImage(imageId, imageFormat, width, height, fileStorageType);
        var account = await _accountRepository.GetByIdAsTrackingAsync(int.Parse(accountId, CultureInfo.InvariantCulture), cancellationToken);

        if (account.Image != null && !string.IsNullOrEmpty(imageId))
        {
            _imageRepository.Delete(account.Image);
        }

        account.UpdateProfile(firstName, lastName, image);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }
}
