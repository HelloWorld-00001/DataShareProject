# Stage 1: Build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the entire solution and restore dependencies

COPY WebApplication1/*.sln .
COPY DataShareAPI/*.csproj ./DataShareAPI/
COPY DataShareCore/*.csproj ./DataShareCore/
COPY DataShareData/*.csproj ./DataShareData/
COPY DataShareTest/*.csproj ./DataShareTest/

RUN dotnet restore

# Copy everything else and build the application
COPY . .
WORKDIR /app/WebApplication1
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port 80 for HTTP traffic
EXPOSE 80

ENTRYPOINT ["dotnet", "WebApplication1.dll"]
