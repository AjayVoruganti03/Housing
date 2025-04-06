# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY WebAPI/*.csproj ./WebAPI/
RUN dotnet restore "./WebAPI/WebAPI.csproj"

# Copy the rest of the files
COPY . ./
WORKDIR /src/WebAPI
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]
