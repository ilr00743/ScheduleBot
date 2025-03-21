using PIBScheduleBot;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var bot = new TelegramBotClient(Environment.GetEnvironmentVariable("BOT_TOKEN"));
var me = await bot.GetMe();

var markupDrawer = new MarkupDrawer();

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{ 
    AllowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery},
    DropPendingUpdates = true
};

bot.StartReceiving(HandleUpdate, HandleError, receiverOptions: receiverOptions, cancellationToken: cts.Token);

await Task.Delay(-1);
async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();

    if (update.Type == UpdateType.Message && update.Message.Text != null)
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
                await bot.SendMessage(update.Message.Chat.Id, text:"Повертаємось до головного меню", replyMarkup: markupDrawer.DrawMainMenu());
                break;
        }
    }
}

Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Помилка: {exception.Message}");
    return Task.CompletedTask;
}

async Task SendScheduleForToday(Update update)
{
    await bot.SendMessage(chatId:update.Message.Chat.Id, text: "Розклад на сьогодні");
}

async Task SendStatusSettings(Update update)
{
    await bot.SendMessage(chatId:update.Message.Chat.Id, text: "Оберіть, хто ви:", replyMarkup: markupDrawer.DrawStatusSettings());
}

async Task SendSuccessfulStatusSetting(Update update)
{
    await bot.SendMessage(update.Message.Chat.Id, text: "Дякуємо за реєстрацію!", replyMarkup: markupDrawer.DrawMainMenu());
}

async Task SendFindByGroup(Update update)
{
    await bot.SendMessage(chatId: update.Message.Chat.Id, text: "Оберіть групу:",
        replyMarkup: markupDrawer.DrawMarkupWithSize<int>(2));
}