package _Self.buildTypes

import jetbrains.buildServer.configs.kotlin.v2018_2.*
import jetbrains.buildServer.configs.kotlin.v2018_2.buildSteps.dotnetBuild
import jetbrains.buildServer.configs.kotlin.v2018_2.buildSteps.dotnetPack
import jetbrains.buildServer.configs.kotlin.v2018_2.failureConditions.BuildFailureOnMetric
import jetbrains.buildServer.configs.kotlin.v2018_2.failureConditions.failOnMetricChange
import jetbrains.buildServer.configs.kotlin.v2018_2.triggers.vcs

object Build : BuildType({
    name = "build"

    artifactRules = "./nupkg"

    vcs {
        root(HttpsGithubComMykquayceHelpersGitRefsHeadsMaster)
    }

    steps {
        dotnetBuild {
            name = "build"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetPack {
            name = "pack DOCKERSECRETS"
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            projects = "Helpers.DockerSecrets/Helpers.DockerSecrets.csproj"
            outputDir = "../nupkg"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetPack {
            name = "pack JAEGER"
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            projects = "Helpers.Jaeger/Helpers.Jaeger.csproj"
            outputDir = "../nupkg"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        dotnetPack {
            name = "pack TRACING"
            executionMode = BuildStep.ExecutionMode.RUN_ON_SUCCESS
            projects = "Helpers.Tracing/Helpers.Tracing.csproj"
            outputDir = "../nupkg"
            param("dotNetCoverage.dotCover.home.path", "%teamcity.tool.JetBrains.dotCover.CommandLineTools.DEFAULT%")
        }
        step {
            name = "deploy"
            type = "ftp-deploy-runner"
            param("jetbrains.buildServer.deployer.ftp.authMethod", "ANONYMOUS")
            param("jetbrains.buildServer.deployer.ftp.transferMethod", "AUTO")
            param("jetbrains.buildServer.deployer.sourcePath", "./nupkg/*.nupkg")
            param("jetbrains.buildServer.deployer.targetUrl", "ftp://nuget/")
            param("jetbrains.buildServer.deployer.ftp.securityMode", "0")
        }
    }

    triggers {
        vcs {
        }
    }

    failureConditions {
        executionTimeoutMin = 1
        errorMessage = true
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.ARTIFACT_SIZE
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.LESS
            compareTo = value()
            param("metricThreshold", "1kb")
            param("anchorBuild", "lastSuccessful")
        }
        failOnMetricChange {
            metric = BuildFailureOnMetric.MetricType.ARTIFACT_SIZE
            units = BuildFailureOnMetric.MetricUnit.DEFAULT_UNIT
            comparison = BuildFailureOnMetric.MetricComparison.MORE
            compareTo = value()
            param("metricThreshold", "1mb")
            param("anchorBuild", "lastSuccessful")
        }
    }
})
