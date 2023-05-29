FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# IMAGE SHOULD BE BUILDED FROM PROJECT source DIRECTORY
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./Services/Ordering/Ordering.Api/Ordering.Api.csproj", "./Services/Ordering/Ordering.Api/"]

# Copy dependencies into image source directory
COPY ["./EventBus/EventBus.Abstractions/", "./EventBus/EventBus.Abstractions/"]
COPY ["./EventBus/EventBus.EventLogs/", "./EventBus/EventBus.EventLogs/"]
COPY ["./EventBus/EventBus.RabbitMQ/", "./EventBus/EventBus.RabbitMQ/"]

COPY ["./Services/Ordering/Ordering.Domain", "./Services/Ordering/Ordering.Domain"]
COPY ["./Services/Ordering/Ordering.Infrastructure", "./Services/Ordering/Ordering.Infrastructure"]

# Build dependencies
RUN dotnet build "./EventBus/EventBus.Abstractions/EventBus.Abstractions.csproj" -c Release
RUN dotnet build "./EventBus/EventBus.EventLogs/EventBus.EventLogs.csproj" -c Release
RUN dotnet build "./EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj" -c Release

RUN dotnet build "./Services/Ordering/Ordering.Domain/Ordering.Domain.csproj" -c Release
RUN dotnet build "./Services/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj" -c Release

# Set workdir that contains project files
WORKDIR /src/Services/Ordering/Ordering.Api/
# Restore the project
RUN dotnet restore "Ordering.Api.csproj"

# Copy project files into destination directory
COPY ./Services/Ordering/Ordering.Api .
# Build project
RUN dotnet build "Ordering.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Ordering.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Set entrypoint
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.Api.dll"]