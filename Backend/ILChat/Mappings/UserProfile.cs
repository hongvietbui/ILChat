using ILChat.Entities;

namespace ILChat.Mappings;

public class UserProfile : MapProfile
{
    public UserProfile()
    {
        CreateMap<KeycloakUser, UserInfo>()
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? ""))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? ""))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username ?? ""))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? ""))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => ""));
    }
}