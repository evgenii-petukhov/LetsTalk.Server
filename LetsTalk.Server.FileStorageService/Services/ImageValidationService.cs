using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.FileStorageService.Models;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorageService.Services;

public class ImageValidationService : IImageValidationService
{
    private const string ImageDimensionsErrorMessage = "Image size exceeds max dimensions";
    private const string ImageFormatErrorMessage = "Image format is not supported";

    private readonly IImageInfoService _imageInfoService;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageValidationService(
        IImageInfoService imageInfoService,
        IOptions<FileStorageSettings> options)
    {
        _imageInfoService = imageInfoService;
        _fileStorageSettings = options.Value;
    }

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
