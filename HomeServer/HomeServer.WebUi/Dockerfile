FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./HomeServer.WebUi/HomeServer.WebUi.csproj"
RUN dotnet build "./HomeServer.WebUi/HomeServer.WebUi.csproj" -c ${configuration} -o /app/build

FROM build AS publish
RUN dotnet publish "./HomeServer.WebUi/HomeServer.WebUi.csproj" -c ${configuration} -o /app/publish /p:UseAppHost=false

FROM base AS final
SHELL ["/bin/bash", "-c"]
WORKDIR /app
COPY --from=publish /app/publish .

CMD ["dotnet", "HomeServer.WebUi.dll"]