FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 5000/TCP
ENV ASPNETCORE_URL https://*:5000
ENTRYPOINT [ "dotnet", "run", "--server.urls", "http://*:5000" ]
