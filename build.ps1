dotnet restore --source https://api.nuget.org/v3/index.json --source https://nuget/v3/index.json `
	|| exit 1


dotnet build --configuration Release --no-restore --verbosity minimal `
	|| exit 1


dotnet pack --configuration Release --no-build --no-restore --nologo --output .\nupkg --runtime linux-x64 --verbosity minimal `
	|| exit 1


if (${env:NuGetServerApiKey} -ne $null) {
	dotnet nuget push .\nupkg\*.nupkg --api-key ${env:NuGetServerApiKey} --skip-duplicate --source https://nuget `
		|| exit 1
}
