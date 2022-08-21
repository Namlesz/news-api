FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["projekt-inzynierski-backend/projekt-inzynierski-backend.csproj", "projekt-inzynierski-backend/"]
RUN dotnet restore "projekt-inzynierski-backend/projekt-inzynierski-backend.csproj"
COPY . .
WORKDIR "/src/projekt-inzynierski-backend"
RUN dotnet build "projekt-inzynierski-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "projekt-inzynierski-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "projekt-inzynierski-backend.dll"]
