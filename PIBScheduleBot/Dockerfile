FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY bin/Release/net9.0 /app

CMD ["dotnet", "PIBScheduleBot.dll"]