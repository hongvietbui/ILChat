using ILChat.Entities.BaseEntities;
using ILChat.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ILChat.Entities;

public class Conversation : MongoBaseEntity
{
    [BsonElement("type")]
    public ConversationType Type { get; set; }
    [BsonElement("participantIds")]
    public List<string> ParticipantIds { get; set; } = [];
    public string? GroupName { get; set; }
}