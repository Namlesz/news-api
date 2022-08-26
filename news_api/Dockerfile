FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["news_api/news_api.csproj", "news_api/"]
RUN dotnet restore "news_api/news_api.csproj"
COPY . .
WORKDIR "/src/news_api"
RUN dotnet build "news_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "news_api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "news_api.dll"]
