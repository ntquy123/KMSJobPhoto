# STAGE 1: Restore and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files to leverage Docker layer caching
COPY KMSJobPhoto.sln ./
COPY erpsolution.api.csproj ./
COPY dal/erpsolution.dal.csproj dal/
COPY entities/erpsolution.entities.csproj entities/
COPY lib/erpsolution.lib.csproj lib/
COPY service/erpsolution.service.csproj service/

# Restore dependencies
RUN dotnet restore "erpsolution.api.csproj"

# Copy the entire source tree and publish the API
COPY . .
RUN dotnet publish "erpsolution.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Configure ASP.NET Core to listen on port 8083
ENV ASPNETCORE_URLS=http://+:8083

# Ensure upload directories exist so bind mounts have predictable targets
RUN mkdir -p /home/data/PKKMS/upload/audit/img /upload

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Document the port the container listens on
EXPOSE 8083

# Run the API
ENTRYPOINT ["dotnet", "erpsolution.api.dll"]
