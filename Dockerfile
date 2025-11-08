FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/Prueba.Bohorquez/*.sln Prueba.Bohorquez/

COPY backend/Prueba.API/*.csproj Prueba.API/
COPY backend/Prueba.Application/*.csproj Prueba.Application/
COPY backend/Prueba.Infrastructure/*.csproj Prueba.Infrastructure/
COPY backend/Prueba.Domain/*.csproj Prueba.Domain/

WORKDIR /src/Prueba.Bohorquez
RUN dotnet restore

WORKDIR /src
COPY backend/Prueba.API Prueba.API/
COPY backend/Prueba.Application Prueba.Application/
COPY backend/Prueba.Infrastructure Prueba.Infrastructure/
COPY backend/Prueba.Domain Prueba.Domain/

WORKDIR "/src/Prueba.API"

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Prueba.API.dll"]
