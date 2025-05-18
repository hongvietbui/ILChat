using ILChat.Entities.BaseEntities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ILChat.Entities;

public class Message : MongoBaseEntity
{
    [BsonElement("senderId")]
    public required string SenderId { get; set; }
    [BsonElement("content")]
    public required string Content { get; set; }
    [BsonElement("seenBy")]
    public Dictionary<string, DateTime> SeenBy { get; set; } = new();
}