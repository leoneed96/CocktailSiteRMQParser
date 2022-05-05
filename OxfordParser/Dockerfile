#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OxfordParser/OxfordParser.csproj", "OxfordParser/"]
COPY ["Oxford.Client/Oxford.Client.csproj", "Oxford.Client/"]
COPY ["OxfordParser.Data/OxfordParser.Data.csproj", "OxfordParser.Data/"]
COPY ["RabbitMQTools/RabbitMQTools.csproj", "RabbitMQTools/"]
COPY ["AngleSharpParser/AngleSharp.Parser.csproj", "AngleSharpParser/"]
RUN dotnet restore "OxfordParser/OxfordParser.csproj"
COPY . .
WORKDIR "/src/OxfordParser"
RUN dotnet build "OxfordParser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OxfordParser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OxfordParser.dll"]
