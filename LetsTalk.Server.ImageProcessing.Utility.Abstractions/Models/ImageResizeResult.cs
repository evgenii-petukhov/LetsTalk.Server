namespace LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;

public readonly record struct ImageResizeResult(byte[]? Data, int Width, int Height);
