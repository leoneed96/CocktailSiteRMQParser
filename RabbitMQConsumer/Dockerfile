#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RabbitMQConsumer/RabbitMQConsumer.csproj", "RabbitMQConsumer/"]
COPY ["Inshaker.Client/Inshaker.Client.csproj", "Inshaker.Client/"]
COPY ["InshakerParser.Data/InshakerParser.Data.csproj", "InshakerParser.Data/"]
COPY ["RabbitMQTools/RabbitMQTools.csproj", "RabbitMQTools/"]
COPY ["AngleSharpParser/AngleSharp.Parser.csproj", "AngleSharpParser/"]
RUN dotnet restore "RabbitMQConsumer/RabbitMQConsumer.csproj"
COPY . .
WORKDIR "/src/RabbitMQConsumer"
RUN dotnet build "RabbitMQConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQConsumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQConsumer.dll"]