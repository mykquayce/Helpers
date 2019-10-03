#! /bin/bash

dotnet build

ls -1 */*.csproj | awk '{system("dotnet pack " $1 " --output ./nupkg")}'

ls -1 ./nupkg/*.nupkg | awk -v ApiKey="$NUGET_SERVER_API_KEY" '{system("nuget push " $1 " " ApiKey " -Source http://nuget/nuget")}'
