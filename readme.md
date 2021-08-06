### ./build.sh ###
Save the Nuget Server API Key to a local variable with:
```bash
export NUGET_SERVER_API_KEY=...
```
### Hierarchies
```
               Common
     +-----------+------+
    Jwt          |    Tracing
                 |  +-----+---+
              RabbitMQ       Web
                              |                          TPLink.Models
     +-----+-------+-----+----+------------+---+------+  |
     |  Discord Elgato GitHub | PhilipsHue | Slack  TPLink
     |                        |            |
     |                        |            |  Steam.Models
     |                        |           Steam
     |  Cineworld.Models      |
 Cineworld                    |  Networking.Models
                              |  +-------+------+
                            OpenWrt Networking SSH
                                         |
                                  +------+-------+
                             GlobalCache XUnitClassFixtures

Telegram.Models
        |
    Telegram
```
### Standalone
```
DawnGuard
DockerSecrets
Jaeger
MySql
Phasmophobia
Reddit.Models
Tracing.Middleware
Twitch
```
### Build order
1. Cineworld.Models, Common, DawnGuard, DockerSecrets, Jaeger, Json, MySql, OldhamCouncil, Phasmophobia, Reddit.Models, Steam.Models, Telegram.Models, TPLink.Models, Tracing.Middleware, and Twitch
1. Jwt, Networking.Models, Reddit, Telegram, and Tracing
1. Networking, RabbitMQ, SSH, and Web
1. Cineworld, Discord, Elgato, GitHub, GlobalCache, Infrared, OpenWrt, PhilipsHue, Slack, Steam, TPLink, and XUnitClassFixtures
### User Secrets
#### Helpers.Discord.Tests
```powershell
dotnet user-secrets set Discord:WebHook:Id ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Discord:WebHook:Token ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.DockerHub.Tests
```powershell
dotnet user-secrets set DockerHub:Password ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set DockerHub:Username ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Elgato.Tests
```bash
dotnet user-secrets set Elgato:EndPoint:IPAddress ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.GlobalCache
```bash
dotnet user-secrets set GlobalCache:HostName ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set GlobalCache:IPAddress ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set GlobalCache:UUID ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.MySql
```bash
dotnet user-secrets set MySql:Username root --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set MySql:Password ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Networking
```bash
dotnet user-secrets set Networking:GlobalCache:BroadcastIPAddress 239.255.250.250 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Networking:GlobalCache:BroadcastPort 4998 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Networking:GlobalCache:HostName iTach059CAD --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Networking:GlobalCache:PhysicalAddress ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set Networking:GlobalCache:ReceivePort 9131 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.OpenWrt
```bash
dotnet user-secrets set OpenWrt:EndPoint 192.168.1.10 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set OpenWrt:Password ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.PhilipsHue
```bash
dotnet user-secrets set PhilipsHue:BridgePhysicalAddress  ...--id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set PhilipsHue:Username ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.RabbitMQ.Tests
```powershell
dotnet user-secrets set RabbitMQSettings:UserName ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set RabbitMQSettings:Password ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.Slack.Tests
```powershell
dotnet user-secrets set Slack:Token xoxp-000000000000-000000000000-000000000000-00000000000000000000000000000000 --id 8391cb70-d94f-4863-b7e4-5659af167bc6
```
#### Helpers.SSH
```powershell
dotnet user-secrets set SSH:Host ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:Port ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:Username ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:Password ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:PathToPublicKey ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
dotnet user-secrets set SSH:PathToPrivateKey ... --id 8391cb70-d94f-4863-b7e4-5659af167bc6
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
### Helpers.Reddit.Models
1. download xml with:
```powesrhell
curl --user-agent "PostmanRuntime/7.26.10" --output .\worldnews.xml https://old.reddit.com/r/worldnews/.rss
```
2. converted to xsd using https://www.freeformatter.com/xsd-generator.html and **Salami Slice**.
1. edit out the fields we don't care about
1. convert to .cs with:
```powershell
& 'C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\xsd.exe' .\worldnews.xsd  /classes /fields /namespace:Helpers.Reddit.Models /out:.
```
