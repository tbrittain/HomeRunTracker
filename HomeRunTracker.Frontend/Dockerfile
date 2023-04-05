FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HomeRunTracker.Frontend/HomeRunTracker.Frontend.csproj", "HomeRunTracker.Frontend/"]
RUN dotnet restore "HomeRunTracker.Frontend/HomeRunTracker.Frontend.csproj"
COPY . .
WORKDIR "/src/HomeRunTracker.Frontend"
RUN dotnet build "HomeRunTracker.Frontend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeRunTracker.Frontend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeRunTracker.Frontend.dll"]
