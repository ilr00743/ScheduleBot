FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY bin/Release/net9.0 /app
COPY appsettings.json /app/appsettings.json

CMD ["dotnet", "Bot.dll"]