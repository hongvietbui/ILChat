using ILChat.Entities;

namespace ILChat.Mappings;

public class ConversationProfile : MapProfile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationDto>().ReverseMap();
    }
}