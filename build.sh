#! /bin/bash

dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json
if [ $? -eq 1 ]; then exit 1; fi

dotnet build --configuration Release --no-restore --verbosity minimal
if [ $? -eq 1 ]; then exit 1; fi

dotnet pack --configuration Release --no-build --no-restore --nologo --output ./nupkg --runtime linux-x64 --verbosity minimal
if [ $? -eq 1 ]; then exit 1; fi

if [ -n "$NuGetServerApiKey" ]
then
	dotnet nuget push ./nupkg/*.nupkg --api-key $NuGetServerApiKey --skip-duplicate --source http://nuget
fi
