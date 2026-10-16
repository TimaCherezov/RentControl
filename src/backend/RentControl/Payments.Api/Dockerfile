FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Payments.Api/Payments.Api.csproj", "Payments.Api/"]
COPY ["Payments.Model/Payments.Model.csproj", "Payments.Model/"]
RUN dotnet restore "Payments.Api/Payments.Api.csproj"
COPY . .
WORKDIR "/src/Payments.Api"
RUN dotnet build "./Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Payments.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payments.Api.dll"]
