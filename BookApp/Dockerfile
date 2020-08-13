#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BookApp/BookApp.csproj", "BookApp/"]
COPY ["DataLayer/DataLayer.csproj", "DataLayer/"]
COPY ["ServiceLayer/ServiceLayer.csproj", "ServiceLayer/"]
COPY ["BizLogic/BizLogic.csproj", "BizLogic/"]
COPY ["BizDbAccess/BizDbAccess.csproj", "BizDbAccess/"]
RUN dotnet restore "BookApp/BookApp.csproj"
COPY . .
WORKDIR "/src/BookApp"
RUN dotnet build "BookApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookApp.dll"]