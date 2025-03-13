using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Tools.ExtractImageInfo;

public class ImageInfoHostedService(
    IImageInfoService imageInfoService,
    TextWriter outputWriter,
    IAgnosticFileService fileService,
    IOptions<FileStorageSettings> options) : IHostedService
{
    private readonly FileStorageSettings _fileStorageSettings = options.Value;
    private readonly IImageInfoService _imageInfoService = imageInfoService;
    private readonly TextWriter _outputWriter = outputWriter;
    private readonly IAgnosticFileService _fileService = fileService;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var imagePath = Path.Combine(
            Environment.ExpandEnvironmentVariables(_fileStorageSettings.BasePath!),
            _fileStorageSettings.ImageFolder!);

        foreach (var filepath in Directory.GetFiles(imagePath).Where(x => string.IsNullOrEmpty(Path.GetExtension(x))))
        {
            var data = await File.ReadAllBytesAsync(filepath, cancellationToken);
            var (width, height) = _imageInfoService.GetImageSize(data);
            await _fileService.SaveImageInfoAsync(filepath, width, height, cancellationToken);
        }

        await _outputWriter.WriteLineAsync("Done");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
