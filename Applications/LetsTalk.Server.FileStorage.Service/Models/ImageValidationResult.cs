using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Models;

public readonly record struct ImageValidationResult(ImageFormats ImageFormat, int Width, int Height);
