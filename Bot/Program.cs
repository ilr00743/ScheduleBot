using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PIBScheduleBot;
using PIBScheduleBot.Services;
using Telegram.Bot;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((services) =>
    {
        var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");

        if (string.IsNullOrEmpty(botToken))
        {
            throw new InvalidOperationException("Bot token is not set!");
        }
        
        Console.WriteLine($"BotToken: {botToken}");
        
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
        services.AddSingleton<MarkupDrawer>();
        services.AddSingleton<UpdateHandler>();
        services.AddHostedService<BotBackgroundService>();
    });

var host = builder.Build();

await host.RunAsync();