using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.Notifications.MappingProfiles;

public class NotificationProfile: Profile
{
	public NotificationProfile()
	{
		CreateMap<LinkPreviewNotification, LinkPreviewNotificationDto>()
			.ForMember(x => x.AccountId, x => x.MapFrom(s => s.RecipientId));
	}
}
