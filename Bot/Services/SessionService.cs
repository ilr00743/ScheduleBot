using Core.Entities;

namespace PIBScheduleBot.Services;

public class SessionService
{
    private readonly Dictionary<string, UserSession> _sessions = new();

    public UserSession GetSession(string userTelegramId)
    {
        if (!_sessions.ContainsKey(userTelegramId))
        {
            _sessions[userTelegramId] = new UserSession();
        }
        
        return _sessions[userTelegramId];
    }

    public void RemoveSession(string userTelegramId)
    {
        if (!_sessions.ContainsKey(userTelegramId))
        {
            return;
        }
        
        _sessions.Remove(userTelegramId);
    }
}