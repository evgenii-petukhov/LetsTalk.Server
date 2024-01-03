using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Tools.ExtractImageInfo;

public class ImageInfoHostedService : IHostedService
{
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly IImageInfoService _imageInfoService;
    private readonly TextWriter _outputWriter;
    private readonly IFileService _fileService;

    public ImageInfoHostedService(
        IImageInfoService imageInfoService,
        TextWriter outputWriter,
        IFileService fileService,
        IOptions<FileStorageSettings> options)
    {
        _fileStorageSettings = options.Value;
        _imageInfoService = imageInfoService;
        _outputWriter = outputWriter;
        _fileService = fileService;
    }

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
