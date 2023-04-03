FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HomeRunTracker.Backend/HomeRunTracker.Backend.csproj", "HomeRunTracker.Backend/"]
RUN dotnet restore "HomeRunTracker.Backend/HomeRunTracker.Backend.csproj"
COPY . .
WORKDIR "/src/HomeRunTracker.Backend"
RUN dotnet build "HomeRunTracker.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeRunTracker.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeRunTracker.Backend.dll"]
