# Imagen base con ASP.NET Core 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Imagen con SDK para compilar la app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Limpiar la caché de NuGet
RUN dotnet nuget locals all --clear

# Copiar y restaurar paquetes
COPY ["ImagesProyect.csproj", "ImagesProyect/"]
WORKDIR "/src/ImagesProyect"
RUN dotnet restore "ImagesProyect.csproj"

# Copiar el código restante y compilar
COPY . .
RUN dotnet restore
RUN dotnet build --configuration Release

# Publicación del proyecto
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ImagesProyect.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final para producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImagesProyect.dll"]
