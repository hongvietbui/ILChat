using ILChat.Entities;

namespace ILChat.Mappings;

public class UserProfile : MapProfile
{
    public UserProfile()
    {
        CreateMap<User, CreateUserInput>().ReverseMap();
        CreateMap<User, GetUserOutput>().ReverseMap();
        CreateMap<User, UpdateUserInput>().ReverseMap();
    }
}