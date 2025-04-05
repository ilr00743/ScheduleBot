namespace Core.DTO;

public class UpdateUserTeacherRequest
{
    public required string TelegramId { get; set; }
    public required int TeacherId { get; set; }
}