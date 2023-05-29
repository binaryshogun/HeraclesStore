FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# IMAGE SHOULD BE BUILDED FROM PROJECT source DIRECTORY
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./Services/Basket/Basket.Api/Basket.Api.csproj", "./Services/Basket/Basket.Api/"]

# Copy dependencies into image source directory
COPY ["./EventBus/EventBus.Abstractions/", "./EventBus/EventBus.Abstractions/"]
COPY ["./EventBus/EventBus.EventLogs/", "./EventBus/EventBus.EventLogs/"]
COPY ["./EventBus/EventBus.RabbitMQ/", "./EventBus/EventBus.RabbitMQ/"]

# Build dependencies
RUN dotnet build "./EventBus/EventBus.Abstractions/EventBus.Abstractions.csproj" -c Release
RUN dotnet build "./EventBus/EventBus.EventLogs/EventBus.EventLogs.csproj" -c Release
RUN dotnet build "./EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj" -c Release

# Set workdir that contains project files
WORKDIR /src/Services/Basket/Basket.Api/
# Restore the project
RUN dotnet restore "Basket.Api.csproj"

# Copy project files into destination directory
COPY ./Services/Basket/Basket.Api .
# Build project
RUN dotnet build "Basket.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Basket.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Set entrypoint
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.Api.dll"]
