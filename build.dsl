package _Self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.dotnetBuild
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.dotnetPack
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.dotnetRestore
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.script
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs

object Build : BuildType({
    name = "build"

    vcs {
        root(HttpsGithubComMykquayceHelpersRefsHeadsMaster)
    }

    steps {
        dotnetRestore {
            args = "--source https://api.nuget.org/v3/index.json --source http://nuget/nuget"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetBuild {
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetPack {
            args = "--output ./nupkg"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        script {
            name = "deploy"
            scriptContent = """ls -1 ./nupkg/*.nupkg | awk '{system("dotnet nuget push " ${'$'}1 " --api-key %system.nuget-server-api-key% --source http://nuget/nuget | head --lines=3")}'"""
        }
    }

    triggers {
        vcs {
        }
    }
})
