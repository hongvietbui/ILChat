using System.ComponentModel.DataAnnotations;
using ILChat.Entities.BaseEntities;

namespace ILChat.Entities;

public class User : BaseEntity<Guid>
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;
    [Required]
    [MaxLength(256)]
    public string Password { get; set; } = null!;
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
    [Required]
    [MaxLength(50)]
    public required string Email { get; set; }
}