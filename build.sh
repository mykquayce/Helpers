#! /bin/bash

dotnet build

ls -1 */*.csproj | awk '{system("dotnet pack " $1 " --output ./nupkg")}'

cp nupkg/*.nupkg //church/nuget/Packages
