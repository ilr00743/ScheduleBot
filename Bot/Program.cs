﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PIBScheduleBot;
using PIBScheduleBot.ApiClients;
using PIBScheduleBot.Services;
using Telegram.Bot;

var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");

        if (string.IsNullOrEmpty(botToken))
        {
            throw new InvalidOperationException("Bot token is not set!");
        }
        
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
        services.AddSingleton<MarkupDrawer>();
        services.AddSingleton<UpdateHandler>();
        services.AddSingleton<SessionService>();
        
        services.AddHostedService<BotBackgroundService>();
        
        services.AddHttpClient<UserApiClient>(client => client.BaseAddress = new Uri("https://advanced-ant-apparent.ngrok-free.app/" ?? throw new ArgumentNullException("ApiKey URL is missing in configuration")));
        services.AddHttpClient<CourseApiClient>(client => client.BaseAddress = new Uri("https://advanced-ant-apparent.ngrok-free.app/" ?? throw new ArgumentNullException("ApiKey URL is missing in configuration")));
        services.AddHttpClient<GroupApiClient>(client => client.BaseAddress = new Uri("https://advanced-ant-apparent.ngrok-free.app/" ?? throw new ArgumentNullException("ApiKey URL is missing in configuration")));
        services.AddHttpClient<DepartmentApiClient>(client => client.BaseAddress = new Uri("https://advanced-ant-apparent.ngrok-free.app/" ?? throw new ArgumentNullException("ApiKey URL is missing in configuration")));
        services.AddHttpClient<TeacherApiClient>(client => client.BaseAddress = new Uri("https://advanced-ant-apparent.ngrok-free.app/" ?? throw new ArgumentNullException("ApiKey URL is missing in configuration")));

    });

var host = builder.Build();

await host.RunAsync();