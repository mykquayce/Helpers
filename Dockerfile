FROM mcr.microsoft.com/dotnet/sdk:10.0

ARG NuGetServerApiKey

WORKDIR /app
COPY . .

RUN dotnet restore --source https://api.nuget.org/v3/index.json --source https://nuget.bob.house/v3/index.json
RUN dotnet build --configuration Release --no-restore --verbosity minimal
RUN dotnet pack --configuration Release --no-build --no-restore --nologo --output /nupkg --verbosity minimal
RUN dotnet nuget push /nupkg/*.nupkg --api-key $NuGetServerApiKey --skip-duplicate --source https://nuget.bob.house
