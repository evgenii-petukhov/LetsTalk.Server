using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.ImageInfo.Models;
using LetsTalk.Server.ImageProcessing.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LetsTalk.Server.Tools.ExtractImageInfo;

public class ImageInfoHostedService : IHostedService
{
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly IImageInfoService _imageInfoService;
    private readonly TextWriter _outputWriter;

    public ImageInfoHostedService(
        IImageInfoService imageInfoService,
        TextWriter outputWriter,
        IOptions<FileStorageSettings> options)
    {
        _fileStorageSettings = options.Value;
        _imageInfoService = imageInfoService;
        _outputWriter = outputWriter;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var imagePath = Path.Combine(
            Environment.ExpandEnvironmentVariables(_fileStorageSettings.BasePath!),
            _fileStorageSettings.ImageFolder!);

        foreach (var filename in Directory.GetFiles(imagePath).Where(x => string.IsNullOrEmpty(Path.GetExtension(x))))
        {
            var data = await File.ReadAllBytesAsync(filename, cancellationToken);
            var (width, height) = _imageInfoService.GetImageSize(data);
            var imageInfo = new ImageInfoModel
            {
                Width = width,
                Height = height
            };

            await File.WriteAllTextAsync(filename + ".info", JsonSerializer.Serialize(imageInfo, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }), cancellationToken);
        }

        await _outputWriter.WriteLineAsync("Done");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
