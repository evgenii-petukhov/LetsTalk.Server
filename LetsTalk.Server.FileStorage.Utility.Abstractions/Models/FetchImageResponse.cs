namespace LetsTalk.Server.FileStorage.Utility.Abstractions.Models;

public readonly record struct FetchImageResponse(byte[]? Content, int Width, int Height);