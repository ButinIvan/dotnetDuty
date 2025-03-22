FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["dotnetWebApi/dotnetWebApi.csproj", "dotnetWebApi/"]
RUN dotnet restore "dotnetWebApi/dotnetWebApi.csproj"

COPY . .

WORKDIR "/src/dotnetWebApi"


ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "dotnetWebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5099
ENTRYPOINT ["dotnet", "dotnetWebApi.dll"]
