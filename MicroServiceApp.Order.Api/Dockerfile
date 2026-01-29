# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MicroServiceApp.Order.Api/MicroServiceApp.Order.Api.csproj", "MicroServiceApp.Order.Api/"]
COPY ["MicroServiceApp.Order.Application/MicroServiceApp.Order.Application.csproj", "MicroServiceApp.Order.Application/"]
COPY ["MicroServiceApp.Bus/MicroServiceApp.Bus.csproj", "MicroServiceApp.Bus/"]
COPY ["MicroServiceApp.Order.Domain/MicroServiceApp.Order.Domain.csproj", "MicroServiceApp.Order.Domain/"]
COPY ["MicroServiceApp.Shared/MicroServiceApp.Shared.csproj", "MicroServiceApp.Shared/"]
COPY ["MicroServiceApp.Order.Persistence/MicroServiceApp.Order.Persistence.csproj", "MicroServiceApp.Order.Persistence/"]
RUN dotnet restore "./MicroServiceApp.Order.Api/MicroServiceApp.Order.Api.csproj"
COPY . .
WORKDIR "/src/MicroServiceApp.Order.Api"
RUN dotnet build "./MicroServiceApp.Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MicroServiceApp.Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroServiceApp.Order.Api.dll"]