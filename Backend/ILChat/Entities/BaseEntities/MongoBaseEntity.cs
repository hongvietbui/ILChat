using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ILChat.Entities.BaseEntities;

public abstract class MongoBaseEntity : IAuditable, IDeletable
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}