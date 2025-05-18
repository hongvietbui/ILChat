using System.ComponentModel.DataAnnotations;
using ILChat.Entities.BaseEntities;

namespace ILChat.Entities;

public class User : BaseEntity<Guid>
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;
    [MaxLength(50)]
    public DateTime? DateOfBirth { get; set; }
    [Required]
    [MaxLength(50)]
    public string Gender { get; set; } = null!;
    [MaxLength(2048)]
    public string? Avatar { get; set; } = null!;
}