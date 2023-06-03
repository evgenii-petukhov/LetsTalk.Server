using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Image.GetImage;

public record GetImageQuery(int Id): IRequest<ImageDto>;