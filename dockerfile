# Build Stage
FROM  mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source 
COPY . .
RUN dotnet restore "./APICore/APICore.API/APICore.API.csproj" --disable-parallel
RUN dotnet publish "./APICore/APICore.API/APICore.API.csproj" -c debug -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000

ENTRYPOINT [ "dotnet", "APICore.API.dll" ]