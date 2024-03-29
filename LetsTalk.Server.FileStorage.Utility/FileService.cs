﻿using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using System.Text.Json;

namespace LetsTalk.Server.FileStorage.Utility;

public class FileService(
    IFileNameGenerator fileNameGenerator,
    IFileStoragePathProvider fileStoragePathProvider) : IFileService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IFileNameGenerator _fileNameGenerator = fileNameGenerator;
    private readonly IFileStoragePathProvider _fileStoragePathProvider = fileStoragePathProvider;

    public async Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default)
    {
        var imagePath = _fileStoragePathProvider.GetFilePath(filename, fileType);
        return File.Exists(imagePath)
            ? await File.ReadAllBytesAsync(imagePath, cancellationToken)
            : [];
    }

    public Task<string> SaveDataAsync(
        byte[] data,
        FileTypes fileType,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        return SaveDataWithRetryAsync(data, () => _fileNameGenerator.Generate(fileType), cancellationToken);
    }

    public Task SaveImageInfoAsync(
        string filename,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var filepath = string.IsNullOrEmpty(Path.GetDirectoryName(filename))
            ? _fileStoragePathProvider.GetFilePath(filename, FileTypes.Image)
            : filename;

        var imageInfo = new ImageInfoModel
        {
            Width = width,
            Height = height
        };

        return File.WriteAllTextAsync(filepath + ".info", JsonSerializer.Serialize(imageInfo, JsonSerializerOptions), cancellationToken);
    }

    public async Task<ImageInfoModel> LoadImageInfoAsync(string filename, CancellationToken cancellationToken = default)
    {
        var imageInfoString = await File.ReadAllTextAsync(filename, cancellationToken);
        return JsonSerializer.Deserialize<ImageInfoModel>(imageInfoString, JsonSerializerOptions)!;
    }

    private static async Task<string> SaveDataWithRetryAsync(
        byte[] data,
        Func<(string, string)> generateUniqueFilename,
        CancellationToken cancellationToken = default)
    {
        var (filename, filepath) = generateUniqueFilename.Invoke();
        try
        {
            await SaveDataWithUniqueNameAsync(data, filepath, cancellationToken);
        }
        catch (IOException)
        {
            (filename, filepath) = generateUniqueFilename.Invoke();
            await SaveDataWithUniqueNameAsync(data, filepath, cancellationToken);
        }
        return filename;
    }

    private static async Task SaveDataWithUniqueNameAsync(
        byte[] data,
        string filepath,
        CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        await stream.WriteAsync(data, cancellationToken);
    }
}
