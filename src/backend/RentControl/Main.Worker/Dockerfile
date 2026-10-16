FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Main.Worker/Main.Worker.csproj", "Main.Worker/"]
COPY ["Main.Model/Main.Model.csproj", "Main.Model/"]
COPY ["RentControl.ServiceDefaults/RentControl.ServiceDefaults.csproj", "RentControl.ServiceDefaults/"]
RUN dotnet restore "Main.Worker/Main.Worker.csproj"
COPY . .
WORKDIR "/src/Main.Worker"
RUN dotnet build "./Main.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Main.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Main.Worker.dll"]
