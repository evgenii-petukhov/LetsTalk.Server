using Amazon.Lambda.Core;
using LetsTalk.Server.FileStorage.Amazon.Services;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine;
using LetsTalk.Server.ImageProcessing.Utility;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;
using LetsTalk.Server.ImageProcessing.Utility.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LetsTalk.Server.ImageProcessing.Lambda
{
    public static class ImageProcessingLambda
    {
        public static Task<ProcessImageResponse> ProcessImageAsync(ProcessImageRequest request)
        {
            var fileService = new AmazonFileService(request.BucketName!);

            var imageProcessingService = new ImageProcessingService(
                fileService,
                new ImageResizeService());

            return imageProcessingService.ProcessImageAsync(request.FileName!, request.MaxWidth, request.MaxHeight);
        }
    }
}
