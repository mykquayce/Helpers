### ./build.sh ###
Save the Nuget Server API Key to a local variable with:
```bash
export NUGET_SERVER_API_KEY=�
```
### User Secrets
#### Helpers.Discord.Tests
```powershell
dotnet user-secrets set Discord:WebHook:Id � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Discord:WebHook:Token � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.DockerHub.Tests
```powershell
dotnet user-secrets set DockerHub:Password � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set DockerHub:Username � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Elgato.Tests
```bash
dotnet user-secrets set Elgato:EndPoint:PhysicalAddress � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Elgato:EndPoint:Port 9123 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.GlobalCache
```bash
dotnet user-secrets set GlobalCache:BroadcastIPAddress � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set GlobalCache:PhysicalAddress � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set GlobalCache:Port � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set GlobalCache:ReceivePort � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.MySql
```bash
dotnet user-secrets set MySql:Username root --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set MySql:Password � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Networking
```bash
dotnet user-secrets set Networking:GlobalCache:BroadcastIPAddress � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Networking:GlobalCache:PhysicalAddress � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Networking:GlobalCache:Port � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Networking:GlobalCache:ReceivePort � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.OpenWrt
```bash
dotnet user-secrets set OpenWrt:EndPoint 192.168.1.10 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set OpenWrt:Password � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.PhilipsHue
```bash
dotnet user-secrets set PhilipsHue:BridgePhysicalAddress  �--id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set PhilipsHue:Username � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.RabbitMQ.Tests
```powershell
dotnet user-secrets set RabbitMQSettings:UserName � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set RabbitMQSettings:Password � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Slack.Tests
```powershell
dotnet user-secrets set Slack:Token xoxp-000000000000-000000000000-000000000000-00000000000000000000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.SSH
```powershell
dotnet user-secrets set SSH:Host � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:Port � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:Username � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:Password � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Steam.Tests
```powershell
dotnet user-secrets set SteamAPI:Key 00000000000000000000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SteamAPI:SteamIds:0 00000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SteamAPI:SteamIds:1 00000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Twitch.Tests
```powershell
dotnet user-secrets set Twitch:Client:BearerToken � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Twitch:Client:Id � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Twitch:Client:Secret � --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.TPLink.Tests
```powershell
dotnet user-secrets set TPLink:UserName ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set TPLink:Password ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
