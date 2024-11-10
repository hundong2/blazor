# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY ./*.csproj ./BlazorAppLee/

# Copy everything else and build
COPY . ./BlazorAppLee/
WORKDIR /source/BlazorAppLee
RUN dotnet publish -c Release -o /app/publish

# Set the working directory for runtime
WORKDIR /app/publish

# Run the application
ENTRYPOINT ["dotnet", "BlazorAppLee.dll", "--urls", "http://0.0.0.0:5001"]