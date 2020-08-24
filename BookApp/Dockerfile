#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app

# Copy the published web app
COPY BookApp/ /app

# Run command
ENTRYPOINT ["dotnet", "BookApp.dll"]

