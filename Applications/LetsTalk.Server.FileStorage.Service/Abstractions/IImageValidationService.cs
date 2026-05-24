using LetsTalk.Server.FileStorage.Service.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Abstractions;

public interface IImageValidationService
{
    ImageValidationResult ValidateImage(byte[] data, ImageRoles imageRole);
}
