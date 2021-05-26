#! /bin/bash

dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json
if [ $? -eq 1 ]; then exit 1; fi

dotnet build --configuration Release --no-restore --verbosity minimal
if [ $? -eq 1 ]; then exit 1; fi

dotnet pack --configuration Release --no-build --no-restore --nologo --output ./nupkg --verbosity minimal
if [ $? -eq 1 ]; then exit 1; fi

if [ -n "$NuGetServerApiKey" ]
then
	for f in ./nupkg/*.nupkg
	do
		dotnet nuget push $f --api-key $NuGetServerApiKey --source http://nuget | head --lines=3
	done
fi
