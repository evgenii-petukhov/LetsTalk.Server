using LetsTalk.Server.FileStorageService.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IImageValidationService
{
    ImageValidationResult ValidateImage(byte[] data, ImageRoles imageRole);
}
