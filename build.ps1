$start = Get-Date -Format "o"

# update the base image
docker pull mcr.microsoft.com/dotnet/sdk:7.0 `
	|| exit 1

# build
docker build `
	--build-arg "NuGetServerApiKey=${env:NuGetServerApiKey}" `
	--no-cache `
	--secret id=ca_crt,src=${env:userprofile}\.aspnet\https\ca.crt `
	.

# remove "dangling" images created since the script began
$ids = docker image ls --filter dangling=true --format '{{.ID}}'

foreach ($id in $ids) {
	$created = docker image inspect --format '{{.Created}}' $id
	if ($created -gt $start) {
		docker image rm $id
	}
}
