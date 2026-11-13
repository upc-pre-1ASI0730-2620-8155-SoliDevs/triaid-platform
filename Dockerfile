# Build stage: restore and publish the API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TriAidPlatform.API/TriAidPlatform.API.csproj", "TriAidPlatform.API/"]
RUN dotnet restore "TriAidPlatform.API/TriAidPlatform.API.csproj"
COPY . .
WORKDIR /src/TriAidPlatform.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage: serve the published API
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
CMD ["sh", "-c", "dotnet TriAidPlatform.API.dll --urls http://0.0.0.0:${PORT:-8080}"]
