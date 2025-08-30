# Imagen base oficial de ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Imagen para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tesina.csproj", "./"]
RUN dotnet restore "Tesina.csproj"
COPY . .
RUN dotnet publish "Tesina.csproj" -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tesina.dll"]
