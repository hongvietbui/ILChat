using ILChat.Utilities;

namespace ILChat.Mappings;

public class PaginationProfile : MapProfile
{
    public PaginationProfile()
    {
        CreateMap(typeof(Pagination<>), typeof(Pagination<>));
    }
}