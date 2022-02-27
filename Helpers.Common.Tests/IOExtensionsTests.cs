using System.Text;
using Xunit;

namespace Helpers.Common.Tests;

public class IOExtensionsTests
{
	public readonly static object[][] Data = new string[][]
	{
			new []
			{
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}\r\n{\"stream\":\"Step 2/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}\r\n{\"stream\":\"Step 3/11 : COPY . .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f62b5baea6f\\n\"}\r\n{\"stream\":\"Step 4/11 : RUN dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 337f31f6162c\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 41.43 sec).\\n\"}\r\n{\"stream\":\"Removing intermediate container 337f31f6162c\\n\"}\r\n{\"stream\":\" ---\\u003e c88470dad6d0\\n\"}\r\n{\"stream\":\"Step 5/11 : RUN dotnet publish DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj --configuration Release --output /app/publish --runtime linux-x64\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 85c7068da127\\n\"}\r\n{\"stream\":\"Microsoft (R) Build Engine version 16.10.0-preview-21126-01+6819f7ab0 for .NET\\r\\nCopyright (C) Microsoft Corporation. All rights reserved.\\r\\n\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 3.17 sec).\\n\"}\r\n{\"stream\":\"  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/DockerSpike2.Services.Tests/bin/Release/net6.0/linux-x64/DockerSpike2.Services.Tests.dll\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/publish/\\n\"}\r\n{\"stream\":\"Removing intermediate container 85c7068da127\\n\"}\r\n{\"stream\":\" ---\\u003e 8208195b5e1c\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:8208195b5e1c9acd56edfd144b55e6d0fdb74ce9518b569514bbb9050bb497b5\"}}\r\n{\"stream\":\"Step 6/11 : FROM mcr.microsoft.com/dotnet/aspnet:6.0\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f7ed34f35d7\\n\"}\r\n{\"stream\":\"Step 7/11 : EXPOSE 80/tcp\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e a3ad13e34d10\\n\"}\r\n{\"stream\":\"Step 8/11 : ENV ASPNETCORE_ENVIRONMENT=Production\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e 4734b674b140\\n\"}\r\n{\"stream\":\"Step 9/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e f9be5b71f70a\\n\"}\r\n{\"stream\":\"Step 10/11 : COPY --from=build-env /app/publish .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 3172ebf9f7c7\\n\"}\r\n{\"stream\":\"Step 11/11 : ENTRYPOINT [\\\"dotnet\\\", \\\"DockerSpike2.Services.Tests.dll\\\"]\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 480e6c511bb3\\n\"}\r\n{\"stream\":\"Removing intermediate container 480e6c511bb3\\n\"}\r\n{\"stream\":\" ---\\u003e 6c8e5ab9952e\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:6c8e5ab9952ef808ec336521259a3b3311261eb181bd9356facbe548886e8ea0\"}}\r\n{\"stream\":\"Successfully built 6c8e5ab9952e\\n\"}\r\n{\"stream\":\"Successfully tagged eassbhhtgu/dockerspike2:latest\\n\"}\r\n",
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}",
			},
			new []
			{
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}\r\n{\"stream\":\"Step 2/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}\r\n{\"stream\":\"Step 3/11 : COPY . .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f62b5baea6f\\n\"}\r\n{\"stream\":\"Step 4/11 : RUN dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 337f31f6162c\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 41.43 sec).\\n\"}\r\n{\"stream\":\"Removing intermediate container 337f31f6162c\\n\"}\r\n{\"stream\":\" ---\\u003e c88470dad6d0\\n\"}\r\n{\"stream\":\"Step 5/11 : RUN dotnet publish DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj --configuration Release --output /app/publish --runtime linux-x64\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 85c7068da127\\n\"}\r\n{\"stream\":\"Microsoft (R) Build Engine version 16.10.0-preview-21126-01+6819f7ab0 for .NET\\r\\nCopyright (C) Microsoft Corporation. All rights reserved.\\r\\n\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 3.17 sec).\\n\"}\r\n{\"stream\":\"  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/DockerSpike2.Services.Tests/bin/Release/net6.0/linux-x64/DockerSpike2.Services.Tests.dll\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/publish/\\n\"}\r\n{\"stream\":\"Removing intermediate container 85c7068da127\\n\"}\r\n{\"stream\":\" ---\\u003e 8208195b5e1c\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:8208195b5e1c9acd56edfd144b55e6d0fdb74ce9518b569514bbb9050bb497b5\"}}\r\n{\"stream\":\"Step 6/11 : FROM mcr.microsoft.com/dotnet/aspnet:6.0\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f7ed34f35d7\\n\"}\r\n{\"stream\":\"Step 7/11 : EXPOSE 80/tcp\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e a3ad13e34d10\\n\"}\r\n{\"stream\":\"Step 8/11 : ENV ASPNETCORE_ENVIRONMENT=Production\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e 4734b674b140\\n\"}\r\n{\"stream\":\"Step 9/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e f9be5b71f70a\\n\"}\r\n{\"stream\":\"Step 10/11 : COPY --from=build-env /app/publish .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 3172ebf9f7c7\\n\"}\r\n{\"stream\":\"Step 11/11 : ENTRYPOINT [\\\"dotnet\\\", \\\"DockerSpike2.Services.Tests.dll\\\"]\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 480e6c511bb3\\n\"}\r\n{\"stream\":\"Removing intermediate container 480e6c511bb3\\n\"}\r\n{\"stream\":\" ---\\u003e 6c8e5ab9952e\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:6c8e5ab9952ef808ec336521259a3b3311261eb181bd9356facbe548886e8ea0\"}}\r\n{\"stream\":\"Successfully built 6c8e5ab9952e\\n\"}\r\n{\"stream\":\"Successfully tagged eassbhhtgu/dockerspike2:latest\\n\"}\r\n",
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}",
				"{\"stream\":\"\\n\"}",
			},
			new[]
			{
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}\r\n{\"stream\":\"Step 2/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}\r\n{\"stream\":\"Step 3/11 : COPY . .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f62b5baea6f\\n\"}\r\n{\"stream\":\"Step 4/11 : RUN dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 337f31f6162c\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 41.43 sec).\\n\"}\r\n{\"stream\":\"Removing intermediate container 337f31f6162c\\n\"}\r\n{\"stream\":\" ---\\u003e c88470dad6d0\\n\"}\r\n{\"stream\":\"Step 5/11 : RUN dotnet publish DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj --configuration Release --output /app/publish --runtime linux-x64\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 85c7068da127\\n\"}\r\n{\"stream\":\"Microsoft (R) Build Engine version 16.10.0-preview-21126-01+6819f7ab0 for .NET\\r\\nCopyright (C) Microsoft Corporation. All rights reserved.\\r\\n\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 3.17 sec).\\n\"}\r\n{\"stream\":\"  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/DockerSpike2.Services.Tests/bin/Release/net6.0/linux-x64/DockerSpike2.Services.Tests.dll\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/publish/\\n\"}\r\n{\"stream\":\"Removing intermediate container 85c7068da127\\n\"}\r\n{\"stream\":\" ---\\u003e 8208195b5e1c\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:8208195b5e1c9acd56edfd144b55e6d0fdb74ce9518b569514bbb9050bb497b5\"}}\r\n{\"stream\":\"Step 6/11 : FROM mcr.microsoft.com/dotnet/aspnet:6.0\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f7ed34f35d7\\n\"}\r\n{\"stream\":\"Step 7/11 : EXPOSE 80/tcp\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e a3ad13e34d10\\n\"}\r\n{\"stream\":\"Step 8/11 : ENV ASPNETCORE_ENVIRONMENT=Production\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e 4734b674b140\\n\"}\r\n{\"stream\":\"Step 9/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e f9be5b71f70a\\n\"}\r\n{\"stream\":\"Step 10/11 : COPY --from=build-env /app/publish .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 3172ebf9f7c7\\n\"}\r\n{\"stream\":\"Step 11/11 : ENTRYPOINT [\\\"dotnet\\\", \\\"DockerSpike2.Services.Tests.dll\\\"]\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 480e6c511bb3\\n\"}\r\n{\"stream\":\"Removing intermediate container 480e6c511bb3\\n\"}\r\n{\"stream\":\" ---\\u003e 6c8e5ab9952e\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:6c8e5ab9952ef808ec336521259a3b3311261eb181bd9356facbe548886e8ea0\"}}\r\n{\"stream\":\"Successfully built 6c8e5ab9952e\\n\"}\r\n{\"stream\":\"Successfully tagged eassbhhtgu/dockerspike2:latest\\n\"}\r\n",
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}",
			},
			new[]
			{
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}\r\n{\"stream\":\"Step 2/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}\r\n{\"stream\":\"Step 3/11 : COPY . .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f62b5baea6f\\n\"}\r\n{\"stream\":\"Step 4/11 : RUN dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 337f31f6162c\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 41.43 sec).\\n\"}\r\n{\"stream\":\"Removing intermediate container 337f31f6162c\\n\"}\r\n{\"stream\":\" ---\\u003e c88470dad6d0\\n\"}\r\n{\"stream\":\"Step 5/11 : RUN dotnet publish DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj --configuration Release --output /app/publish --runtime linux-x64\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 85c7068da127\\n\"}\r\n{\"stream\":\"Microsoft (R) Build Engine version 16.10.0-preview-21126-01+6819f7ab0 for .NET\\r\\nCopyright (C) Microsoft Corporation. All rights reserved.\\r\\n\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 3.17 sec).\\n\"}\r\n{\"stream\":\"  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/DockerSpike2.Services.Tests/bin/Release/net6.0/linux-x64/DockerSpike2.Services.Tests.dll\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/publish/\\n\"}\r\n{\"stream\":\"Removing intermediate container 85c7068da127\\n\"}\r\n{\"stream\":\" ---\\u003e 8208195b5e1c\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:8208195b5e1c9acd56edfd144b55e6d0fdb74ce9518b569514bbb9050bb497b5\"}}\r\n{\"stream\":\"Step 6/11 : FROM mcr.microsoft.com/dotnet/aspnet:6.0\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f7ed34f35d7\\n\"}\r\n{\"stream\":\"Step 7/11 : EXPOSE 80/tcp\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e a3ad13e34d10\\n\"}\r\n{\"stream\":\"Step 8/11 : ENV ASPNETCORE_ENVIRONMENT=Production\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e 4734b674b140\\n\"}\r\n{\"stream\":\"Step 9/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e f9be5b71f70a\\n\"}\r\n{\"stream\":\"Step 10/11 : COPY --from=build-env /app/publish .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 3172ebf9f7c7\\n\"}\r\n{\"stream\":\"Step 11/11 : ENTRYPOINT [\\\"dotnet\\\", \\\"DockerSpike2.Services.Tests.dll\\\"]\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 480e6c511bb3\\n\"}\r\n{\"stream\":\"Removing intermediate container 480e6c511bb3\\n\"}\r\n{\"stream\":\" ---\\u003e 6c8e5ab9952e\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:6c8e5ab9952ef808ec336521259a3b3311261eb181bd9356facbe548886e8ea0\"}}\r\n{\"stream\":\"Successfully built 6c8e5ab9952e\\n\"}\r\n{\"stream\":\"Successfully tagged eassbhhtgu/dockerspike2:latest\\n\"}\r\n",
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}",
				"{\"stream\":\"Step 2/11 : WORKDIR /app\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Using cache\\n\"}",
				"{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}",
				"{\"stream\":\"Step 3/11 : COPY . .\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e 6f62b5baea6f\\n\"}",
				"{\"stream\":\"Step 4/11 : RUN dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Running in 337f31f6162c\\n\"}",
				"{\"stream\":\"  Determining projects to restore...\\n\"}",
				"{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 41.43 sec).\\n\"}",
				"{\"stream\":\"Removing intermediate container 337f31f6162c\\n\"}",
				"{\"stream\":\" ---\\u003e c88470dad6d0\\n\"}",
				"{\"stream\":\"Step 5/11 : RUN dotnet publish DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj --configuration Release --output /app/publish --runtime linux-x64\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Running in 85c7068da127\\n\"}",
				"{\"stream\":\"Microsoft (R) Build Engine version 16.10.0-preview-21126-01+6819f7ab0 for .NET\\r\\nCopyright (C) Microsoft Corporation. All rights reserved.\\r\\n\\n\"}",
				"{\"stream\":\"  Determining projects to restore...\\n\"}",
				"{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 3.17 sec).\\n\"}",
				"{\"stream\":\"  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview\\n\"}",
				"{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/DockerSpike2.Services.Tests/bin/Release/net6.0/linux-x64/DockerSpike2.Services.Tests.dll\\n\"}",
				"{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/publish/\\n\"}",
				"{\"stream\":\"Removing intermediate container 85c7068da127\\n\"}",
				"{\"stream\":\" ---\\u003e 8208195b5e1c\\n\"}",
				"{\"aux\":{\"ID\":\"sha256:8208195b5e1c9acd56edfd144b55e6d0fdb74ce9518b569514bbb9050bb497b5\"}}",
				"{\"stream\":\"Step 6/11 : FROM mcr.microsoft.com/dotnet/aspnet:6.0\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e 6f7ed34f35d7\\n\"}",
				"{\"stream\":\"Step 7/11 : EXPOSE 80/tcp\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Using cache\\n\"}",
				"{\"stream\":\" ---\\u003e a3ad13e34d10\\n\"}",
				"{\"stream\":\"Step 8/11 : ENV ASPNETCORE_ENVIRONMENT=Production\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Using cache\\n\"}",
				"{\"stream\":\" ---\\u003e 4734b674b140\\n\"}",
				"{\"stream\":\"Step 9/11 : WORKDIR /app\"}", "{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Using cache\\n\"}",
				"{\"stream\":\" ---\\u003e f9be5b71f70a\\n\"}",
				"{\"stream\":\"Step 10/11 : COPY --from=build-env /app/publish .\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e 3172ebf9f7c7\\n\"}",
				"{\"stream\":\"Step 11/11 : ENTRYPOINT [\\\"dotnet\\\", \\\"DockerSpike2.Services.Tests.dll\\\"]\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Running in 480e6c511bb3\\n\"}",
				"{\"stream\":\"Removing intermediate container 480e6c511bb3\\n\"}",
				"{\"stream\":\" ---\\u003e 6c8e5ab9952e\\n\"}",
				"{\"aux\":{\"ID\":\"sha256:6c8e5ab9952ef808ec336521259a3b3311261eb181bd9356facbe548886e8ea0\"}}",
				"{\"stream\":\"Successfully built 6c8e5ab9952e\\n\"}",
				"{\"stream\":\"Successfully tagged eassbhhtgu/dockerspike2:latest\\n\"}",
			},
			new[]
			{
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}\r\n{\"stream\":\"Step 2/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}\r\n{\"stream\":\"Step 3/11 : COPY . .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f62b5baea6f\\n\"}\r\n{\"stream\":\"Step 4/11 : RUN dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 337f31f6162c\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 41.43 sec).\\n\"}\r\n{\"stream\":\"Removing intermediate container 337f31f6162c\\n\"}\r\n{\"stream\":\" ---\\u003e c88470dad6d0\\n\"}\r\n{\"stream\":\"Step 5/11 : RUN dotnet publish DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj --configuration Release --output /app/publish --runtime linux-x64\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 85c7068da127\\n\"}\r\n{\"stream\":\"Microsoft (R) Build Engine version 16.10.0-preview-21126-01+6819f7ab0 for .NET\\r\\nCopyright (C) Microsoft Corporation. All rights reserved.\\r\\n\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 3.17 sec).\\n\"}\r\n{\"stream\":\"  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/DockerSpike2.Services.Tests/bin/Release/net6.0/linux-x64/DockerSpike2.Services.Tests.dll\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/publish/\\n\"}\r\n{\"stream\":\"Removing intermediate container 85c7068da127\\n\"}\r\n{\"stream\":\" ---\\u003e 8208195b5e1c\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:8208195b5e1c9acd56edfd144b55e6d0fdb74ce9518b569514bbb9050bb497b5\"}}\r\n{\"stream\":\"Step 6/11 : FROM mcr.microsoft.com/dotnet/aspnet:6.0\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f7ed34f35d7\\n\"}\r\n{\"stream\":\"Step 7/11 : EXPOSE 80/tcp\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e a3ad13e34d10\\n\"}\r\n{\"stream\":\"Step 8/11 : ENV ASPNETCORE_ENVIRONMENT=Production\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e 4734b674b140\\n\"}\r\n{\"stream\":\"Step 9/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e f9be5b71f70a\\n\"}\r\n{\"stream\":\"Step 10/11 : COPY --from=build-env /app/publish .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 3172ebf9f7c7\\n\"}\r\n{\"stream\":\"Step 11/11 : ENTRYPOINT [\\\"dotnet\\\", \\\"DockerSpike2.Services.Tests.dll\\\"]\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 480e6c511bb3\\n\"}\r\n{\"stream\":\"Removing intermediate container 480e6c511bb3\\n\"}\r\n{\"stream\":\" ---\\u003e 6c8e5ab9952e\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:6c8e5ab9952ef808ec336521259a3b3311261eb181bd9356facbe548886e8ea0\"}}\r\n{\"stream\":\"Successfully built 6c8e5ab9952e\\n\"}\r\n{\"stream\":\"Successfully tagged eassbhhtgu/dockerspike2:latest\\n\"}\r\n",
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}",
				"{\"stream\":\"\\n\"}", "{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}",
				"{\"stream\":\"Step 2/11 : WORKDIR /app\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e Using cache\\n\"}",
				"{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}",
				"{\"stream\":\"Step 3/11 : COPY . .\"}",
			},
			new[]
			{
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}\r\n{\"stream\":\"Step 2/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}\r\n{\"stream\":\"Step 3/11 : COPY . .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f62b5baea6f\\n\"}\r\n{\"stream\":\"Step 4/11 : RUN dotnet restore --source https://api.nuget.org/v3/index.json --source http://nuget/v3/index.json\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 337f31f6162c\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 41.43 sec).\\n\"}\r\n{\"stream\":\"Removing intermediate container 337f31f6162c\\n\"}\r\n{\"stream\":\" ---\\u003e c88470dad6d0\\n\"}\r\n{\"stream\":\"Step 5/11 : RUN dotnet publish DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj --configuration Release --output /app/publish --runtime linux-x64\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 85c7068da127\\n\"}\r\n{\"stream\":\"Microsoft (R) Build Engine version 16.10.0-preview-21126-01+6819f7ab0 for .NET\\r\\nCopyright (C) Microsoft Corporation. All rights reserved.\\r\\n\\n\"}\r\n{\"stream\":\"  Determining projects to restore...\\n\"}\r\n{\"stream\":\"  Restored /app/DockerSpike2.Services.Tests/DockerSpike2.Services.Tests.csproj (in 3.17 sec).\\n\"}\r\n{\"stream\":\"  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/DockerSpike2.Services.Tests/bin/Release/net6.0/linux-x64/DockerSpike2.Services.Tests.dll\\n\"}\r\n{\"stream\":\"  DockerSpike2.Services.Tests -\\u003e /app/publish/\\n\"}\r\n{\"stream\":\"Removing intermediate container 85c7068da127\\n\"}\r\n{\"stream\":\" ---\\u003e 8208195b5e1c\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:8208195b5e1c9acd56edfd144b55e6d0fdb74ce9518b569514bbb9050bb497b5\"}}\r\n{\"stream\":\"Step 6/11 : FROM mcr.microsoft.com/dotnet/aspnet:6.0\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 6f7ed34f35d7\\n\"}\r\n{\"stream\":\"Step 7/11 : EXPOSE 80/tcp\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e a3ad13e34d10\\n\"}\r\n{\"stream\":\"Step 8/11 : ENV ASPNETCORE_ENVIRONMENT=Production\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e 4734b674b140\\n\"}\r\n{\"stream\":\"Step 9/11 : WORKDIR /app\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Using cache\\n\"}\r\n{\"stream\":\" ---\\u003e f9be5b71f70a\\n\"}\r\n{\"stream\":\"Step 10/11 : COPY --from=build-env /app/publish .\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e 3172ebf9f7c7\\n\"}\r\n{\"stream\":\"Step 11/11 : ENTRYPOINT [\\\"dotnet\\\", \\\"DockerSpike2.Services.Tests.dll\\\"]\"}\r\n{\"stream\":\"\\n\"}\r\n{\"stream\":\" ---\\u003e Running in 480e6c511bb3\\n\"}\r\n{\"stream\":\"Removing intermediate container 480e6c511bb3\\n\"}\r\n{\"stream\":\" ---\\u003e 6c8e5ab9952e\\n\"}\r\n{\"aux\":{\"ID\":\"sha256:6c8e5ab9952ef808ec336521259a3b3311261eb181bd9356facbe548886e8ea0\"}}\r\n{\"stream\":\"Successfully built 6c8e5ab9952e\\n\"}\r\n{\"stream\":\"Successfully tagged eassbhhtgu/dockerspike2:latest\\n\"}\r\n",
				"{\"stream\":\"Step 1/11 : FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env\"}",
				"{\"stream\":\"\\n\"}",
				"{\"stream\":\" ---\\u003e 297b62e075cd\\n\"}",
				"{\"stream\":\"Step 2/11 : WORKDIR /app\"}",
				"{\"stream\":\"\\n\"}", "{\"stream\":\" ---\\u003e Using cache\\n\"}",
				"{\"stream\":\" ---\\u003e bbbb062c0180\\n\"}",
				"{\"stream\":\"Step 3/11 : COPY . .\"}",
				"{\"stream\":\"\\n\"}",
			},
	};

	[Theory]
	[MemberData(nameof(Data))]
	public void Stream_ReadLines(string response, params string[] expected)
	{
		// Arrange
		var bytes = Encoding.UTF8.GetBytes(response);
		using var stream = new MemoryStream(bytes);

		// Act
		var actual = IOExtensions.ReadLines(stream).Take(expected.Length).ToList();

		// Assert
		Assert.Equal(expected.Length, actual.Count);
		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(Data))]
	public async Task Stream_ReadLinesAsync(string response, params string[] expected)
	{
		// Arrange
		var bytes = Encoding.UTF8.GetBytes(response);
		await using var stream = new MemoryStream(bytes);

		// Act
		var actual = await IOExtensions.ReadLinesAsync(stream).Take(expected.Length).ToListAsync();

		// Assert
		Assert.Equal(expected.Length, actual.Count);
		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(Data))]
	public void StreamReader_ReadLines(string response, params string[] expected)
	{
		// Arrange
		var bytes = Encoding.UTF8.GetBytes(response);
		using var stream = new MemoryStream(bytes);
		var reader = new StreamReader(stream);

		// Act
		var actual = IOExtensions.ReadLines(reader).Take(expected.Length).ToList();

		// Assert
		Assert.Equal(expected.Length, actual.Count);
		Assert.Equal(expected, actual);
	}

	[Theory]
	[MemberData(nameof(Data))]
	public async Task StreamReader_ReadLinesAsync(string response, params string[] expected)
	{
		// Arrange
		var bytes = Encoding.UTF8.GetBytes(response);
		await using var stream = new StreamWrapper(new MemoryStream(bytes));
		var reader = new StreamReader(stream);

		// Act
		var actual = await IOExtensions.ReadLinesAsync(stream).Take(expected.Length).ToListAsync();

		// Assert
		Assert.False(stream.Disposed);
		Assert.Equal(expected.Length, actual.Count);
		Assert.Equal(expected, actual);
	}

	public sealed class StreamWrapper : Stream, IDisposable, IAsyncDisposable
	{
		private readonly Stream _stream;

		public StreamWrapper(Stream stream)
		{
			_stream = stream
				?? throw new ArgumentNullException(nameof(stream));
		}

		#region Stream implementation
		public override bool CanRead => _stream.CanRead;
		public override bool CanSeek => _stream.CanSeek;
		public override bool CanWrite => _stream.CanWrite;
		public override long Length => _stream.Length;
		public override long Position { get => _stream.Position; set => _stream.Position = value; }
		public override void Flush() => _stream.Flush();
		public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);
		public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
		public override void SetLength(long value) => _stream.SetLength(value);
		public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);
		#endregion Stream implementation

		public bool Disposed { get; private set; }

		public new void Dispose()
		{
			Disposed = true;
			_stream.Dispose();
		}

		public new ValueTask DisposeAsync()
		{
			Disposed = true;
			return _stream.DisposeAsync();
		}

		public static explicit operator StreamWrapper(MemoryStream stream) => new(stream);
	}

	[Theory]
	[InlineData("./data/username", ".", "data", "username")]
	[InlineData(@".\data\username", ".", "data", "username")]
	public void FixPathTests(string before, params string[] expected)
	{
		var actual = before.Split('/', '\\');
		Assert.Equal(expected, actual);
	}
}
