FROM mcr.microsoft.com/dotnet/core/sdk:3.1.201-alpine3.11 AS build-env
WORKDIR /app
EXPOSE 80

# Copy csproj and restore as distinct layers
COPY ./MakersPortal.sln ./MakersPortal.sln
COPY ./MakersPortal.Core/MakersPortal.Core.csproj ./MakersPortal.Core/MakersPortal.Core.csproj
COPY ./MakersPortal.Infrastructure/MakersPortal.Infrastructure.csproj ./MakersPortal.Infrastructure/MakersPortal.Infrastructure.csproj
COPY ./MakersPortal.Tests.Unit/MakersPortal.Tests.Unit.csproj ./MakersPortal.Tests.Unit/MakersPortal.Tests.Unit.csproj
COPY ./MakersPortal.Tests.Integration/MakersPortal.Tests.Integration.csproj ./MakersPortal.Tests.Integration/MakersPortal.Tests.Integration.csproj
COPY ./MakersPortal.WebApi/MakersPortal.WebApi.csproj ./MakersPortal.WebApi/MakersPortal.WebApi.csproj
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish "./MakersPortal.Core/MakersPortal.Core.csproj" -c Release -o out
RUN dotnet publish "./MakersPortal.Infrastructure/MakersPortal.Infrastructure.csproj" -c Release -o out
RUN dotnet publish "./MakersPortal.WebApi/MakersPortal.WebApi.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.3-alpine3.11
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MakersPortal.WebApi.dll"]