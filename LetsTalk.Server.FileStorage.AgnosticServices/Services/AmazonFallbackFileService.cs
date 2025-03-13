using Amazon.S3;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.AgnosticServices.Services;

public class AmazonFallbackFileService(
    IFileService fileService,
    IAmazonFileService amazonFileService) : IAgnosticFileService
{
    private readonly IFileService _fileService = fileService;
    private readonly IAmazonFileService _amazonFileService = amazonFileService;

    public async Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _amazonFileService.ReadFileAsync(filename, fileType, cancellationToken);
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return await _fileService.ReadFileAsync(filename, fileType, cancellationToken);
        }
    }

    public Task<string> SaveDataAsync(
        byte[] data,
        FileTypes fileType,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        return _amazonFileService.SaveDataAsync(data, fileType, width, height, cancellationToken);
    }

    public Task SaveImageInfoAsync(
        string filename,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        return _amazonFileService.SaveImageInfoAsync(filename, width, height, cancellationToken);
    }

    public async Task<ImageInfoModel> LoadImageInfoAsync(string filename, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _amazonFileService.LoadImageInfoAsync(filename, cancellationToken);
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return await _fileService.LoadImageInfoAsync(filename, cancellationToken);
        }
    }

    public void DeleteFile(string filename, FileTypes fileType)
    {
        throw new NotImplementedException();
    }
}
