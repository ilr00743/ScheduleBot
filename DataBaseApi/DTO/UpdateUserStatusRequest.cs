using Core.Entities;

namespace DataBaseApi.DTO;

public class UpdateUserStatusRequest
{
    public required string TelegramId { get; set; }
    public required UserStatus UserStatus { get; set; }
}