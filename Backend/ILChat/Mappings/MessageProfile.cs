using Google.Protobuf.WellKnownTypes;
using ILChat.Entities;

namespace ILChat.Mappings;

public class MessageProfile : MapProfile
{
    public MessageProfile()
    {
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Timestamp,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(DateTime.SpecifyKind(src.CreatedAt, DateTimeKind.Utc))))
            .ReverseMap()
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.Timestamp.ToDateTime()));
    }
}