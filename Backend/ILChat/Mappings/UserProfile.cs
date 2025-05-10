using ILChat.Entities;

namespace ILChat.Mappings;

public class UserProfile : MapProfile
{
    public UserProfile()
    {
        CreateMap<User, CreateUserRequest>().ReverseMap();
        CreateMap<User, GetUserResponse>().ReverseMap();
        CreateMap<User, UpdateUserRequest>().ReverseMap();
    }
}