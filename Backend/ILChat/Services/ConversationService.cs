using Grpc.Core;
using ILChat.Enums;
using ILChat.Repositories.IRepositories;
using MongoDB.Bson;
using ILChat.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ILChat.Services;

public class ConversationService(IMongoRepository<Conversation> conversationRepo, IMongoRepository<MessageDto> messageRepo) : ILChat.ConversationService.ConversationServiceBase
{
    public override Task<FetchConversationsResponse> FetchConversations(FetchConversationsRequest request, ServerCallContext context)
    {
        return base.FetchConversations(request, context);
    }

    public override Task<SearchUsersResponse> SearchUsers(SearchUsersRequest request, ServerCallContext context)
    {
        return base.SearchUsers(request, context);
    }

    [Authorize]
    public override async Task<CreateConversationResponse> CreateConversation(CreateConversationRequest request, ServerCallContext context)
    {
        if(request.ParticipantIds.Count < 2)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "At least two participants are required"));
        
        if(!Enum.TryParse<ConversationType>(request.Type, true, out var type))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid conversation type"));

        var conversation = new Conversation
        {
            Id = ObjectId.GenerateNewId().ToString(),
            ParticipantIds = request.ParticipantIds.ToList(),
            Type = type,
            GroupName = type == ConversationType.Group ? request.GroupName ?? "New Group" : null
        };

        await conversationRepo.AddAsync(conversation);
        
        return new CreateConversationResponse { ConversationId = conversation.Id };
    }
}