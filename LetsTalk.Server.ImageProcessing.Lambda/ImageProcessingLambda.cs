using Amazon.Lambda.Core;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine;
using LetsTalk.Server.ImageProcessing.Utility;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;
using LetsTalk.Server.ImageProcessing.Utility.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LetsTalk.Server.ImageProcessing.Lambda
{
    public static class ImageProcessingLambda
    {
        public static Task<ProcessImageResponse> ProcessImageAsync(ProcessImageRequest request)
        {
            var imageProcessingService = new ImageProcessingService(
                new FakeFileServiceResolver(request.BucketName!),
                new ImageResizeService());

            return imageProcessingService.ProcessImageAsync(request.FileName!, request.MaxWidth, request.MaxHeight);
        }
    }
}
