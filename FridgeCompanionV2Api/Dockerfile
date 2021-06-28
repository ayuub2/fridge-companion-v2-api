#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FridgeCompanionV2Api/FridgeCompanionV2Api.csproj", "FridgeCompanionV2Api/"]
RUN dotnet restore "FridgeCompanionV2Api/FridgeCompanionV2Api.csproj"
COPY . .
WORKDIR "/src/FridgeCompanionV2Api"
RUN dotnet build "FridgeCompanionV2Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FridgeCompanionV2Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FridgeCompanionV2Api.dll", "--server.urls", "http://*:80" ]