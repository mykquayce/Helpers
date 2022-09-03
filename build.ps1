docker pull mcr.microsoft.com/dotnet/sdk:7.0 `
	|| exit 1

docker build `
	--build-arg "NuGetServerApiKey=${env:NuGetServerApiKey}" `
	--no-cache `
	--secret id=ca_crt,src=${env:userprofile}\.aspnet\https\ca.crt `
	.

docker image prune --force
