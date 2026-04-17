# STAGE 1: Build the application
# We use the SDK (Software Development Kit) because it has the compiler.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies (NuGet packages)
# This is a separate step to make building faster in the cloud
COPY *.csproj ./
RUN dotnet restore

# Copy every other file (Controllers, Services, etc.) and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# STAGE 2: Run the application
# We switch to a "Runtime" image. It's much smaller because it 
# doesn't contain the compiler—only what's needed to run the app.
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the compiled "ready-to-run" files from the build stage
COPY --from=build /app/out .

# Tell the container to listen on port 8080 (standard for cloud hosting)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# The command that starts your API
ENTRYPOINT ["dotnet", "api.dll"]