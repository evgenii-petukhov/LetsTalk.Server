namespace LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions.Models;

public readonly record struct ImageResizeResult(byte[]? Data, int Width, int Height);
