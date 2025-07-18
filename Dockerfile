# Usa la imagen oficial de .NET SDK para construir
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia los archivos CSPROJ y restaura dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia todo y construye la app
COPY . ./
RUN dotnet publish -c Release -o out

# Usa la imagen oficial de .NET Runtime para correr
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expón el puerto que Render usa
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Blog.dll"]
