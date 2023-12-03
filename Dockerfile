FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG NuGetServerApiKey

RUN --mount=type=secret,id=ca_crt,dst=/usr/local/share/ca-certificates/ca.crt \
	/usr/sbin/update-ca-certificates

WORKDIR /app
COPY . .

RUN dotnet restore --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json
RUN dotnet build --configuration Release --no-restore --verbosity minimal
RUN dotnet pack --configuration Release --no-build --no-restore --nologo --output /nupkg --runtime linux-x64 --verbosity minimal
RUN dotnet nuget push /nupkg/*.nupkg --api-key $NuGetServerApiKey --skip-duplicate --source https://nuget
