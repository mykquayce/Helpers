package _Self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2018_2.*
import jetbrains.buildServer.configs.kotlin.v2018_2.buildSteps.dotnetBuild
import jetbrains.buildServer.configs.kotlin.v2018_2.buildSteps.dotnetPack
import jetbrains.buildServer.configs.kotlin.v2018_2.buildSteps.dotnetRestore
import jetbrains.buildServer.configs.kotlin.v2018_2.buildSteps.script
import jetbrains.buildServer.configs.kotlin.v2018_2.triggers.vcs

object Build : BuildType({
    name = "Build"

    vcs {
        root(HttpsGithubComMykquayceHelpersGitRefsHeadsMaster)
    }

    steps {
        dotnetRestore {
            name = "restore"
            args = "-s https://api.nuget.org/v3/index.json -s http://nuget/nuget"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetBuild {
            name = "build"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetPack {
            name = "pack"
            outputDir = "./nupkg"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        script {
            name = "push"
            scriptContent = """
                NuGetServerApiKey=${'$'}(cat /run/secrets/NuGetServerApiKey)
                
                for f in ./nupkg/*.nupkg
                do
                  dotnet nuget push ${'$'}f --api-key ${'$'}NuGetServerApiKey --source http://nuget/nuget
                done
                
                return 0
            """.trimIndent()
        }
    }

    triggers {
        vcs {
        }
    }
})
