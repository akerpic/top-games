FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TopGames/TopGames.csproj", "TopGames/"]
RUN dotnet restore "TopGames/TopGames.csproj"
COPY . .
WORKDIR "/src/TopGames"
RUN dotnet build "TopGames.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TopGames.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TopGames.dll"]
