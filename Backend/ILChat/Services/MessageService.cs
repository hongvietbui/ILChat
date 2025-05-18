using AutoMapper;
using Grpc.Core;
using ILChat.Entities;
using ILChat.Repositories.IRepositories;
using MongoDB.Driver;

namespace ILChat.Services;

public class MessageService(
    IMongoRepository<Conversation> conversationRepo,
    IMongoRepository<Message> messageRepo,
    IMapper mapper)
    : ILChat.MessageService.MessageServiceBase
{
    public override async Task<FetchMessagesResponse> FetchMessages(FetchMessagesRequest request,
        ServerCallContext context)
    {
        if (!string.IsNullOrEmpty(request.ConversationId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Conversation ID is required."));

        var conversation = await conversationRepo.GetByIdAsync(request.ConversationId);
        if (conversation == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Conversation not found."));

        var filter = Builders<Message>.Filter.Eq("ConversationId", request.ConversationId);
        var sort = Builders<Message>.Sort.Ascending(m => m.CreatedAt);
        var messages = await messageRepo.FindAsync(filter, sort);

        var response = new FetchMessagesResponse();
        response.Messages.AddRange(mapper.Map<List<MessageDto>>(messages));

        return response;
    }

    public override Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
    {
        //Add message to conversation
        //Send message to user
        return base.SendMessage(request, context);
    }
}