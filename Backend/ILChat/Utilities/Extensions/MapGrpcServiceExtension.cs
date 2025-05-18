namespace ILChat.Utilities.Extensions;
using ILChat.Services;

public static class MapGrpcServiceExtension
{
    public static WebApplication AddMapGrpcServiceExtension(this WebApplication app)
    {
        app.MapGrpcService<MessageService>();
        app.MapGrpcService<ConversationService>();
        app.MapGrpcService<UserService>();
        return app;
    }
}