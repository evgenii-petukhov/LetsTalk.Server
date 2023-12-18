using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.ImageProcessing.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IO;

namespace LetsTalk.Server.Tools.ImageInfoTool;

public class ImageInfoHostedService : IHostedService
{
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly IImageInfoService _imageInfoService;

    public ImageInfoHostedService(
        IImageInfoService imageInfoService,
        IOptions<FileStorageSettings> options)
    {
        _fileStorageSettings = options.Value;
        _imageInfoService = imageInfoService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var imagePath = Path.Combine(
            Environment.ExpandEnvironmentVariables(_fileStorageSettings.BasePath!),
            _fileStorageSettings.ImageFolder!);

        foreach (var filename in Directory.GetFiles(imagePath))
        {
            var data = File.ReadAllBytes(Path.Combine(imagePath, filename));
            _imageInfoService.GetImageSize(data);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
