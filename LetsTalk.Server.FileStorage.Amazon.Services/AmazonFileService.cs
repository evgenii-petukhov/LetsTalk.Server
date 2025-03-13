using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace LetsTalk.Server.FileStorage.Amazon.Services;

public class AmazonFileService : IFileService
{
    private readonly IFileService? _fileService;
    private readonly string? _accessKey;
    private readonly string? _secretKey;
    private readonly string? _region;
    private readonly string _bucketName;
    private readonly bool _isInitializedByLambda;

    public AmazonFileService(
        IFileService fileService,
        IOptions<AwsSettings> options)
    {
        _fileService = fileService;
        _accessKey = options.Value.AccessKey!;
        _secretKey = options.Value.SecretKey!;
        _region = options.Value.Region!;
        _bucketName = options.Value.BucketName!;
    }

    public AmazonFileService(string bucketName)
    {
        _isInitializedByLambda = true;
        _bucketName = bucketName;
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = filename
        };

        using var client = GetS3Client();
        try
        {
            using var response = await client.GetObjectAsync(request, cancellationToken);
            await using var ms = new MemoryStream();
            await response.ResponseStream.CopyToAsync(ms, cancellationToken);
            return ms.ToArray();
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound && _fileService != null)
        {
            return await _fileService.ReadFileAsync(filename, fileType, cancellationToken);
        }
    }

    public async Task<string> SaveDataAsync(
        byte[] data,
        FileTypes fileType,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        using var client = GetS3Client();
        using var transferUtility = new TransferUtility(client);
        await using var ms = new MemoryStream(data);
        var filename = Guid.NewGuid().ToString();
        await transferUtility.UploadAsync(ms, _bucketName, filename, cancellationToken);
        return filename;
    }

    public async Task SaveImageInfoAsync(
        string filename,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var imageInfo = new ImageInfoModel
        {
            Width = width,
            Height = height
        };

        var data = Encoding.Default.GetBytes(JsonSerializer.Serialize(imageInfo, JsonSerializerOptions));

        using var client = GetS3Client();
        using var transferUtility = new TransferUtility(client);
        await using var ms = new MemoryStream(data);
        await transferUtility.UploadAsync(ms, _bucketName, filename + ".info", cancellationToken);
    }

    public async Task<ImageInfoModel> LoadImageInfoAsync(string filename, CancellationToken cancellationToken = default)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = filename
        };

        using var client = GetS3Client();
        try
        {
            using var response = await client.GetObjectAsync(request, cancellationToken);
            using var reader = new StreamReader(response.ResponseStream);
            var imageInfoString = await reader.ReadToEndAsync(cancellationToken);
            return JsonSerializer.Deserialize<ImageInfoModel>(imageInfoString, JsonSerializerOptions)!;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound && _fileService != null)
        {
            return await _fileService.LoadImageInfoAsync(filename, cancellationToken);
        }
    }

    private AmazonS3Client GetS3Client()
    {
        if (_isInitializedByLambda)
        {
            return new AmazonS3Client();
        }

        var awsCredentials = new BasicAWSCredentials(_accessKey, _secretKey);
        var s3Config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_region)
        };
        return new AmazonS3Client(awsCredentials, s3Config);
    }

    public void DeleteFile(string filename, FileTypes fileType)
    {
        throw new NotImplementedException();
    }
}
