namespace ILChat.Entities.BaseEntities;

public interface IDeletable
{
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}