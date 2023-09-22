namespace LetsTalk.Server.ImageProcessing.Abstractions.Models;

public readonly record struct ImageResizeResult(byte[]? Data, int Width, int Height);
