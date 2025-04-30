using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class User
{
    [Key, Editable(false), Display(AutoGenerateField = false)]
    public int Id { get; set; }
    public required string TelegramId { get; set; }
    public required string UserName { get; set; }
    public required UserStatus Status { get; set; }
    
    [Display(AutoGenerateField = false)]
    public int? GroupId { get; set; }
    
    [ForeignKey(nameof(GroupId))]
    public Group? Group { get; set; }
    
    [Display(AutoGenerateField = false)]
    public int? TeacherId { get; set; }

    [ForeignKey(nameof(TeacherId))]
    public Teacher? Teacher { get; set; }
}

public enum UserStatus
{
    None = 0,
    Student = 1,
    Teacher = 2
}