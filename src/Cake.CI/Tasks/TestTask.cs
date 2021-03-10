using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Coverlet;
using Cake.Frosting;

namespace Cake.CI.Tasks
{
    [TaskName("Test")]
    [IsDependentOn(typeof(BuildTask))]
    public sealed class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information($"Working directory: {context.Environment.WorkingDirectory.FullPath}");

            var coverletSettings = new CoverletSettings
            {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover,
                CoverletOutputDirectory = context.Environment.WorkingDirectory.Combine(new DirectoryPath("reports")),
                CoverletOutputName = "coverage",
                Verbosity = DotNetCoreVerbosity.Diagnostic,
                DiagnosticOutput = true,
            };

            context.DotNetCoreTest(
                "../Monitor.sln",
                new DotNetCoreTestSettings
                {
                    Settings = "./../MongoDbMonitorTest/runsettings.xml",
                    Configuration = context.MsBuildConfiguration,
                    ResultsDirectory = context.Environment.WorkingDirectory.Combine(new DirectoryPath("reports")),
                    NoBuild = true,
                    DiagnosticOutput = true,
                    Verbosity = DotNetCoreVerbosity.Normal
                },
                coverletSettings);
        }
    }
}
