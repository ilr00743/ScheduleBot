namespace Core.DTO;

public class UpdateUserGroupRequest
{
    public required string TelegramId { get; set; }
    public required int GroupId { get; set; }
}