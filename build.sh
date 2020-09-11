#! /bin/bash

dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json

dotnet build

#for p in ./*/*.csproj
#do
	#dotnet pack $p --output ./nupkg
#done

dotnet pack --output ./nupkg

if [ -n "$NuGetServerApiKey" ]
then
	for f in ./nupkg/*.nupkg
	do
		dotnet nuget push $f --api-key $NuGetServerApiKey --source http://nuget | head --lines=3
	done
fi
