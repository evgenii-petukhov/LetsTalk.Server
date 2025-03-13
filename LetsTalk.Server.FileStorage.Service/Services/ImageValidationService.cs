using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Models;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorage.Service.Services;

public class ImageValidationService(
    IImageInfoService imageInfoService,
    IOptions<FileStorageSettings> options) : IImageValidationService
{
    private const string ImageDimensionsErrorMessage = "Image size exceeds max dimensions";
    private const string ImageFormatErrorMessage = "Image format is not supported";

    private readonly IImageInfoService _imageInfoService = imageInfoService;
    private readonly FileStorageSettings _fileStorageSettings = options.Value;

    public ImageValidationResult ValidateImage(byte[] data, ImageRoles imageRole)
    {
        var imageFormat = _imageInfoService.GetImageFormat(data);

        if (imageFormat != ImageFormats.Webp)
        {
            throw new BadRequestException(ImageFormatErrorMessage);
        }

        (int width, int height) = _imageInfoService.GetImageSize(data);
        if (imageRole == ImageRoles.Avatar && (width > _fileStorageSettings.AvatarMaxWidth || height > _fileStorageSettings.AvatarMaxHeight))
        {
            throw new ImageSizeException(ImageDimensionsErrorMessage);
        }

        if (imageRole == ImageRoles.Message && (width > _fileStorageSettings.PictureMaxWidth || height > _fileStorageSettings.PictureMaxHeight))
        {
            throw new ImageSizeException(ImageDimensionsErrorMessage);
        }

        return new ImageValidationResult
        {
            ImageFormat = imageFormat,
            Width = width,
            Height = height
        };
    }
}
