#! /bin/bash

dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/nuget

dotnet build

dotnet pack --output ./nupkg

if [ -n "$NUGET_SERVER_API_KEY" ]
then
	for f in ./nupkg/*.nupkg
	do
		dotnet nuget push $f --api-key $NUGET_SERVER_API_KEY --source http://nuget | head --lines=3
	done
fi
