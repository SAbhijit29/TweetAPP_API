#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
#COPY ["TweetAPP/TweetAPP.csproj", "TweetAPP/"]

COPY *.csproj ./
RUN dotnet restore "TweetAPP/TweetAPP.csproj"
COPY . ./

#WORKDIR "/src/TweetAPP"
RUN dotnet build "TweetAPP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TweetAPP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TweetAPP.dll"]







