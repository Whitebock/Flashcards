FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/command-handler/src/CommandHandler.csproj", "services/command-handler/src/"]
COPY ["libraries/common/src/Common.csproj", "libraries/common/src/"]
RUN dotnet restore "services/command-handler/src/CommandHandler.csproj"
COPY . .
WORKDIR "/src/services/command-handler/src"
RUN dotnet build "./CommandHandler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommandHandler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommandHandler.dll"]
