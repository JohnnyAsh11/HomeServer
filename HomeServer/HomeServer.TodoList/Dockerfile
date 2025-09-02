FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./HomeServer.TodoList/HomeServer.TodoList.csproj"
RUN dotnet build "./HomeServer.TodoList/HomeServer.TodoList.csproj" -c ${configuration} -o /app/build

FROM build AS publish
RUN dotnet publish "./HomeServer.TodoList/HomeServer.TodoList.csproj" -c ${configuration} -o /app/publish /p:UseAppHost=false

FROM base AS final
SHELL ["/bin/bash", "-c"]
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./HomeServer.OpenApi/openapi.yaml /app/wwwroot/swagger
COPY ./HomeServer.TodoList/run.sh /app
RUN chmod +x ./run.sh

CMD ["./run.sh"]