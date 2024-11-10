# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY ./*.csproj ./BlazorAppLee/
RUN dotnet restore

# Copy everything else and build
COPY BlazorAppLee/. ./BlazorAppLee/
WORKDIR /source/BlazorAppLee
RUN dotnet publish -c Release -o /app/publish