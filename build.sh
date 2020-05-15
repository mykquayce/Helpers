#! /bin/bash

dotnet restore -s https://api.nuget.org/v3/index.json -s http://nuget/nuget

dotnet build

dotnet pack --output ./nupkg

ls -1 ./nupkg/*.nupkg \
	| awk -v ApiKey="$NUGET_SERVER_API_KEY" '{system("dotnet nuget push " $1 " --api-key " ApiKey " --source http://nuget/nuget \
	| head --lines=3")}'
