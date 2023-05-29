FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# IMAGE SHOULD BE BUILDED FROM PROJECT source DIRECTORY
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./Services/Catalog/Catalog.Api/Catalog.Api.csproj", "./Services/Catalog/Catalog.Api/"]

# Copy dependencies into image source directory
COPY ["./EventBus/EventBus.Abstractions/", "./EventBus/EventBus.Abstractions/"]
COPY ["./EventBus/EventBus.EventLogs/", "./EventBus/EventBus.EventLogs/"]
COPY ["./EventBus/EventBus.RabbitMQ/", "./EventBus/EventBus.RabbitMQ/"]

# Build dependencies
RUN dotnet build "./EventBus/EventBus.Abstractions/EventBus.Abstractions.csproj" -c Release
RUN dotnet build "./EventBus/EventBus.EventLogs/EventBus.EventLogs.csproj" -c Release
RUN dotnet build "./EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj" -c Release

# Set workdir that contains project files
WORKDIR /src/Services/Catalog/Catalog.Api/
# Restore the project
RUN dotnet restore "Catalog.Api.csproj"

# Copy project files into destination directory
COPY ./Services/Catalog/Catalog.Api .
# Build project
RUN dotnet build "Catalog.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Catalog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Set entrypoint
FROM base AS final
WORKDIR /app
RUN mkdir ./Pictures
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
