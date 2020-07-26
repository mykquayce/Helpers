### ./build.sh ###
Save the Nuget Server API Key to a local variable with:
```bash
export NUGET_SERVER_API_KEY=…
```
### User Secrets
#### Helpers.DockerHub.Tests
```
dotnet user-secrets set DockerHub:Password … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set DockerHub:Username … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Slack.Tests
```
dotnet user-secrets set Slack:Token xoxp-000000000000-000000000000-000000000000-00000000000000000000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Steam.Tests
```
dotnet user-secrets set SteamAPI:Key 00000000000000000000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SteamAPI:SteamIds:0 00000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SteamAPI:SteamIds:1 00000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Twitch.Tests
```
dotnet user-secrets set Twitch:Client:BearerToken ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Twitch:Client:Id ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Twitch:Client:Secret ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
