# Використовуємо офіційний образ .NET 8 для запуску
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копіюємо збудовані файли з попереднього етапу
COPY bin/Release/net9.0 /app

# Запускаємо бота
CMD ["dotnet", "PIBScheduleBot.dll"]