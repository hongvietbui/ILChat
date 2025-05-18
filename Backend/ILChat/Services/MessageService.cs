using Grpc.Core;
using ILChat.Repositories.IRepositories;

namespace ILChat.Services;

public class MessageService(IMongoRepository<ConversationDto> conversationRepo, IMongoRepository<MessageDto> messageRepo) : ILChat.MessageService.MessageServiceBase
{
    public override Task<FetchMessagesResponse> FetchMessages(FetchMessagesRequest request, ServerCallContext context)
    {
        return base.FetchMessages(request, context);
    }

    public override Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
    {
        //Add conversation if not exists
        //Add message to conversation
        //Send message to user
        return base.SendMessage(request, context);
    }
}