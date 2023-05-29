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
COPY ["./Bff/Bff.Web/Bff.Web.csproj", "./Bff/Bff.Web/Bff.Web.csproj"]

# Set workdir that contains project files
WORKDIR /src/Bff/Bff.Web
# Restore the project
RUN dotnet restore "Bff.Web.csproj"

# Copy project files into destination directory
COPY ./Bff/Bff.Web .
# Build project
RUN dotnet build "Bff.Web.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Bff.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Set entrypoint
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bff.Web.dll"]