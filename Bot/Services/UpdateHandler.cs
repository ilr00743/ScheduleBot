using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PIBScheduleBot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly MarkupDrawer _markupDrawer;
    
    public UpdateHandler(ITelegramBotClient botClient, MarkupDrawer markupDrawer)
    {
        _botClient = botClient;
        _markupDrawer = markupDrawer;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            switch (update.Message.Text)
            {
                case "/start":
                    await SendStatusSettings(update);
                    break;
            
                case "Студент":
                case "Викладач":
                    await SendSuccessfulStatusSetting(update);
                    break;
            
                case "\ud83d\udccb Розклад на сьогодні":
                    await SendScheduleForToday(update);
                    break;
            
                case "\ud83d\udccb Розклад на наступний день":
                    Console.WriteLine("Розклад на наступний день");
                    break;
            
                case "\u26a0\ufe0f Зміни на сьогодні":
                    Console.WriteLine("Зміни на сьогодні");
                    break;
            
                case "\u26a0\ufe0f Зміни на наступний день":
                    Console.WriteLine("Зміни на наступний день");
                    break;
            
                case "#\ufe0f\u20e3 Пошук за групою":
                    await SendFindByGroup(update);
                    break;
            
                case "\ud83e\uddd1\u200d\ud83c\udfeb Пошук за викладачем":
                    break;
            
                case "\ud83d\udcc5 Пошук за днем тижня":
                    break;
                case "Головне меню":
                    await _botClient.SendMessage(update.Message.Chat.Id, text:"Повертаємось до головного меню", replyMarkup: _markupDrawer.DrawMainMenu(), cancellationToken: cancellationToken);
                    break;
            }
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Помилка: {exception.Message}");
        return Task.CompletedTask;
    }

    private async Task SendScheduleForToday(Update update)
    {
        await _botClient.SendMessage(chatId:update.Message.Chat.Id, text: "Розклад на сьогодні");
    }

    private async Task SendStatusSettings(Update update)
    {
        await _botClient.SendMessage(chatId:update.Message.Chat.Id, text: "Оберіть, хто ви:", replyMarkup: _markupDrawer.DrawStatusSettings());
    }

    private async Task SendSuccessfulStatusSetting(Update update)
    {
        await _botClient.SendMessage(update.Message.Chat.Id, text: "Дякуємо за реєстрацію!", replyMarkup: _markupDrawer.DrawMainMenu());
    }

    private async Task SendFindByGroup(Update update)
    {
        await _botClient.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть групу:",
            replyMarkup: _markupDrawer.DrawMarkupWithSize<int>(2));
    }
}