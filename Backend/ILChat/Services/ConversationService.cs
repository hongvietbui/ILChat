using AutoMapper;
using Grpc.Core;
using ILChat.Enums;
using ILChat.Repositories.IRepositories;
using MongoDB.Bson;
using ILChat.Entities;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace ILChat.Services;

[Authorize]
public class ConversationService(
    IMongoRepository<Conversation> conversationRepo,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper) : ILChat.ConversationService.ConversationServiceBase
{
    public override async Task<FetchConversationsResponse> FetchConversations(FetchConversationsRequest request,
        ServerCallContext context)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if(user == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        
        var userId = user.FindFirst("sub")?.Value;
        
        var filter = Builders<Conversation>.Filter.AnyEq(c => c.ParticipantIds, userId);
        var conversations = await conversationRepo.FindAsync(filter);
        var mappedConversation = mapper.Map<List<ConversationDto>>(conversations);
        
        var response = new FetchConversationsResponse();

        response.Conversations.AddRange(mappedConversation);
        
        return response;
    }

    public override async Task<CreateConversationResponse> CreateConversation(CreateConversationRequest request,
        ServerCallContext context)
    {
        if (request.ParticipantIds.Count < 2)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "At least two participants are required"));

        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirst("sub")?.Value;
        
        if(!request.ParticipantIds.Contains(userId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "User must be a participant"));

        if (!Enum.TryParse<ConversationType>(request.Type, true, out var type))
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