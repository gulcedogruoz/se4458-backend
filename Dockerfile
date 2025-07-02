# ===========================
# HotelBookingSystem Dockerfile
# ===========================

# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY HotelBookingSystem/*.csproj ./HotelBookingSystem/
RUN dotnet restore

# Copy everything else and build
COPY HotelBookingSystem/. ./HotelBookingSystem/
WORKDIR /app/HotelBookingSystem
RUN dotnet publish -c Release -o out

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/HotelBookingSystem/out ./

# Expose port (change if different in Program.cs)
EXPOSE 5177

# Run the application
ENTRYPOINT ["dotnet", "HotelBookingSystem.dll"]
