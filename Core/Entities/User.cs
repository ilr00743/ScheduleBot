namespace Core.Entities;

public class User
{
    public int Id { get; set; }
    public required string TelegramId { get; set; }
    public required string UserName { get; set; }
    public required UserStatus Status { get; set; }
    
    public int? GroupId { get; set; }
    public Group? Group { get; set; }
    
    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}

public enum UserStatus
{
    None = 0,
    Student = 1,
    Teacher = 2
}