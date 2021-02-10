### ./build.sh ###
Save the Nuget Server API Key to a local variable with:
```bash
export NUGET_SERVER_API_KEY=…
```
### User Secrets
#### Helpers.Discord.Tests
```powershell
dotnet user-secrets set Discord:WebHook:Id … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Discord:WebHook:Token … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.DockerHub.Tests
```powershell
dotnet user-secrets set DockerHub:Password … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set DockerHub:Username … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Elgato.Tests
```bash
dotnet user-secrets set Elgato:EndPoint 192.168.1.218:9123 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.OpenWrt
```bash
dotnet user-secrets set OpenWrt:EndPoint 192.168.1.10 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set OpenWrt:Password … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.PhilipsHue
```bash
dotnet user-secrets set PhilipsHue:BridgeIPAddress 192.168.1.156 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set PhilipsHue:Username … --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Slack.Tests
```powershell
dotnet user-secrets set Slack:Token xoxp-000000000000-000000000000-000000000000-00000000000000000000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Steam.Tests
```powershell
dotnet user-secrets set SteamAPI:Key 00000000000000000000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SteamAPI:SteamIds:0 00000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SteamAPI:SteamIds:1 00000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Twitch.Tests
```powershell
dotnet user-secrets set Twitch:Client:BearerToken ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Twitch:Client:Id ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Twitch:Client:Secret ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.TPLink.Tests
```powershell
dotnet user-secrets set TPLink:UserName ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set TPLink:Password ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
